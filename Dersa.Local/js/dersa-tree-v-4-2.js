class DersaNode {
  constructor(id, stereotype, name, has_children = false, tree = null) {
    this.id = id;
    this.stereotype = stereotype;
    this.name = name;
    this.has_children = has_children;
    this._parent = null;
    this._children = [];
    // ссылка на дерево, чтобы при добавлении ребёнка сразу класть его в dTree.nodes
    this._tree = tree;
    if(tree) {
      tree.getProperties(id)
        .then(result => {
					this._properties = result;				
				});;
    }
  }

  parent() {
    return this._parent;
  }

  children() {
    return [...this._children];
  }

  setParent(parentNode) {
    this._parent = parentNode;
  }

  properties() {
    return this._properties;
  }

  
  /**
   * Добавить дочерний узел.
   * Если у текущего узла есть ссылка на дерево (_tree), новый узел сразу регистрируется в нём.
   */
  addChild(childNode) {
    childNode._tree = this._tree;
    this._children.push(childNode);
    childNode.setParent(this);

    // Если у нас есть ссылка на дерево — регистрируем ребёнка в нём
    if (this._tree) {
      this._tree.registerNode(childNode);
    }
  }

  generateId(tree, childStereotype) {
    const parentId = this.id;
    return tree.generateNewNodeId(parentId, childStereotype);
  }

  getSaveRootId() {
    if (this.stereotype === 'Package' || this.stereotype === 'SuperPackage') {
      return '#';
    }

    let current = this._parent;
    while (current) {
      if (current.stereotype === 'Package') {
        return current.id;
      }
      current = current._parent;
    }

    throw new Error(`Не найден родительский узел со стереотипом "Package" для узла ${this.id}. Проверьте структуру дерева.`);
  }


  getPackageDataForSave() {
    if (this.stereotype !== 'Package') {
      throw new Error(`Метод prepareData можно вызывать только для узлов со стереотипом Package. Текущий узел: ${this.id} (${this.stereotype})`);
    }

    const buildDto = (node) => {
      const dto = {
        id: node.id,
        stereotype: node.stereotype,
        name: node.name,
        children: []
      };

      for (const child of node.children()) {
        // Фильтруем по префиксам P и D на каждом уровне рекурсии
        if (typeof child.id === 'string' && (child.id.startsWith('P') || child.id.startsWith('D'))) {
          dto.children.push(buildDto(child));
        }
      }

      return dto;
    };

    const result = [];

    for (const child of this.children()) {
      // На верхнем уровне тоже фильтруем только узлы с префиксами P или D
      if (typeof child.id === 'string' && (child.id.startsWith('P') || child.id.startsWith('D'))) {
        result.push(buildDto(child));
      }
    }

    return result;
  }

  deleteFromDatabase(dbManager){
    dbManager.deleteData("attributes", this.id);
    if(this.stereotype === 'Package')
      dbManager.deleteData("entities", this.id);
  }
}



class DersaTree {
  constructor(dbManager) {
    this.dbManager = dbManager || null;
    this.nodes = new Map();
    this.relations = null;
  }

  getNode(node_id) {
    return this.nodes.get(node_id) || null;
  }

deleteNode(id) {
  const node = this.nodes.get(id);
  if (!node) return;

  if(!node._parent){
    alert('Незльзя удалять корневой узел');
    return;
  }

  // Сначала собираем всех потомков (включая вложенные)
  const descendants = [];
  const collectDescendants = (curr) => {
    for (const child of curr.children()) {
      descendants.push(child);
      collectDescendants(child);
    }
  };
  collectDescendants(node);

  // Удаляем потомков из Map
  for (const desc of descendants) {
    this.nodes.delete(desc.id);
    desc.deleteFromDatabase(this.dbManager);
  }

  // Удаляем сам узел из Map
  this.nodes.delete(id);

  // Убираем узел из children родителя, если он есть
  if (node._parent) {
    const parent = node._parent;
    const idx = parent._children.findIndex(c => c.id === id);
    if (idx !== -1) {
      parent._children.splice(idx, 1);
    }
    // Сбрасываем родителя у удалённого узла
    node.setParent(null);
  }

  node.deleteFromDatabase(this.dbManager);

}

  /**
   * Регистрирует узел в дереве (добавляет в Map).
   * Вызывается автоматически из addChild, если у узла есть ссылка на дерево.
   */
  registerNode(node) {
    if (!node || !node.id) {
      throw new Error('Не удалось зарегистрировать узел: отсутствует node или node.id');
    }
    this.nodes.set(node.id, node);
  }

  /**
   * Получить максимальный номер для P-узлов (тип : каталог, содержащий сущности, Package).
   * ID имеют вид: P1, P2, P3, ...
   */
  getMaxPackageNumber() {
    let maxNum = 0;

    for (const node of this.nodes.values()) {
      if (node.stereotype === 'Package') {
        const match = node.id.match(/^P(\d+)$/);
        if (match) {
          const num = parseInt(match[1], 10);
          if (num > maxNum) {
            maxNum = num;
          }
        }
      }
    }

    return maxNum;
  }

  /**
   * Получить следующий индекс для дочернего узла родителя (для типов 1 и 3).
   * Ищем среди детей родителя ID вида "<parentId>_<число>" и берём max + 1.
   */
  getNextChildIndex(parentId) {
    const parentNode = this.getNode(parentId);
    if (!parentNode) {
      return 1;
    }

    let maxIdx = 0;

    for (const child of parentNode.children()) {
      const pattern = `^${parentId}_(\\d+)$`;
      const regex = new RegExp(pattern);
      const match = child.id.match(regex);
      if (match) {
        const idx = parseInt(match[1], 10);
        if (idx > maxIdx) {
          maxIdx = idx;
        }
      }
    }

    return maxIdx + 1;
  }

  /**
   * Сгенерировать новый ID для узла по родителю и стереотипу.
   * - Тип 2 (Package): P1, P2, P3, ...
   * - Типы 1 (SuperPackage) и 3 (любые другие): <parentId>_1, <parentId>_2, ...
   */
  generateNewNodeId(parentId, stereotype) {
    // Тип 2 теперь — 'Package'
    const type2Stereotype = 'Package';

    if (stereotype === type2Stereotype) {
      // Для каталога, содержащего сущности: L<номер>
      const nextNum = this.getMaxPackageNumber() + 1;
      return `P${nextNum}`;
    } else {
      // Для SuperPackage (тип 1) и всех остальных (тип 3)
      if (!parentId) {
        // Корневой узел без родителя — отдельная логика (подстройте при необходимости)
        return String(this.nodes.size + 1);
      }
      const nextIdx = this.getNextChildIndex(parentId);
      return `${parentId}_${nextIdx}`;
    }
  }
  // -------------------------------------

  async getDiagramData(node_id){
    const diagramData = await this.dbManager.getData('diagrams', node_id);
    return diagramData;
  }

  async saveDiagramData(diagramId, xml){
    await this.dbManager.saveData('diagrams', diagramId, {xml: xml});
    console.log('diagram saved');
  }

getDataForSave(nodeId) {
  if (nodeId === '#') {
    const rootNodes = [];
    for (const [, node] of this.nodes.entries()) {
      if (!node._parent) {
        rootNodes.push(node);
      }
    }

    const buildDto = (node) => {
      // Для Package — используем has_children по приоритету из поля узла, иначе проверяем _children
      if (node.stereotype === 'Package') {
        const hasChildrenFlag = node.has_children === true
          ? true
          : node._children.length > 0;

        return {
          id: node.id,
          stereotype: node.stereotype,
          name: node.name,
          has_children: hasChildrenFlag
        };
      }

      // Для SuperPackage и остальных — оставляем children с фильтрацией
      const dto = {
        id: node.id,
        stereotype: node.stereotype,
        name: node.name,
        children: []
      };

      for (const child of node.children()) {
        // Оставляем только SuperPackage и Package
        if (child.stereotype === 'SuperPackage' || child.stereotype === 'Package') {
          dto.children.push(buildDto(child));
        }
      }

      return dto;
    };

    const result = [];
    for (const root of rootNodes) {
      result.push(buildDto(root));
    }

    return result;
  }

  const node = this.getNode(nodeId);
  if (!node) {
    throw new Error(`Узел с id "${nodeId}" не найден`);
  }

  return node.getPackageDataForSave();
}

  async saveNodeData(nodeId){
    await this.dbManager.saveData('entities', nodeId, this.getDataForSave(nodeId));
    console.log(`node ${nodeId} saved`);
  }

async saveAllNodes() {
  const nodesToSave = [];

  // Проходим по всем узлам в Map и собираем те, у которых stereotype === 'Package'
  for (const [, node] of this.nodes.entries()) {
    if (node.stereotype === 'Package') {
      nodesToSave.push(node);
    }
  }

  // Сохраняем каждый найденный Package-узел
  for (const node of nodesToSave) {
    await this.saveNodeData(node.id);
  }
  console.log(`Сохранено ${nodesToSave.length} узлов со стереотипом Package`);
  this.saveNodeData('#')
  console.log(`Сохранена информация по SuperPackage`);

}

  async loadRelations() {
    const records = await this.dbManager.getAllData('relations');
    const result = records.map(item => {
      if (typeof item.data === 'object' && item.data !== null) {
        item.data["relation_id"] = 'R' + item.id;	
        return item.data;
      }
      return item.data;
    });
    return result;
  }

  getRelation(relation_id) {
    const relation = this.relations.find(r => r.relation_id === relation_id);
    return relation || null;
  }

  getARelations(node_id) {
    return this.relations
      .filter(r => r.aNodeId === node_id)
      .map(r => ({
        relation_id: r.relation_id,
        stereotype: r.stereotype,
        bNodeId: r.bNodeId
      }));
  }

  getBRelations(node_id) {
    return this.relations
      .filter(r => r.bNodeId === node_id)
      .map(r => ({
        relation_id: r.relation_id,
        aNodeId: r.aNodeId
      }));
  }

  async getChildrenJSON(parent_id) {
    if (parent_id === '#') {
      return JSON.stringify([
        { id: 1, stereotype: 'Package', name: 'root', has_children: false }
      ]);
    }
    return '[]';
  }

  async loadNodes(parent_id) {
    if (!Array.isArray(this.relations)) {
      this.relations = await this.loadRelations();
    }

    try {
      const childrenData = await this.dbManager.getData('entities', parent_id);
      const parentNode = parent_id === '#' ? null : this.getNode(parent_id);

      for (const nodeData of childrenData) {
        // Передаём this (само дерево) в конструктор узла
        const node = new DersaNode(
          nodeData.id,
          nodeData.stereotype,
          nodeData.name,
          nodeData.has_children || false,
          this
        );

        // Регистрируем узел в дереве ДО того, как будем добавлять детей
        this.registerNode(node);

        if (parentNode) {
          parentNode.addChild(node);
        }

        if (Array.isArray(nodeData.children) && nodeData.children.length > 0) {
          for (const childData of nodeData.children) {
            const childNode = new DersaNode(
              childData.id,
              childData.stereotype,
              childData.name,
              childData.has_children || false,
              this
            );
            this.registerNode(childNode);
            node.addChild(childNode);

            if (Array.isArray(childData.children) && childData.children.length > 0) {
              await this._loadSubtree(childNode, childData.children);
            }
          }
        }

        const aRelations = this.getARelations(nodeData.id);
        if (Array.isArray(aRelations) && aRelations.length > 0) {
          for (const R of aRelations) {
            const relNode = new DersaNode(R.relation_id, R.stereotype, R.bNodeId, false, this);
            this.registerNode(relNode);
            node.addChild(relNode);
          }
        }
      }

      return childrenData.map(node => node.id);
    } catch (error) {
      console.error(`Ошибка при загрузке узлов для parent_id ${parent_id}:`, error);
      throw error;
    }
  }

  async _loadSubtree(parentNode, childrenData) {
    for (const childData of childrenData) {
      const childNode = new DersaNode(
        childData.id,
        childData.stereotype,
        childData.name,
        childData.has_children || false,
        this
      );
      this.registerNode(childNode);
      parentNode.addChild(childNode);

      if (Array.isArray(childData.children) && childData.children.length > 0) {
        await this._loadSubtree(childNode, childData.children);
      }
    }
  }


  async buildDisplayNode(node) {
    const displayNode = {
      id: node.id,
      text: node.name,
      icon: node.stereotype
    };

    const actualChildren = node.children();

    if (actualChildren.length > 0) {
      displayNode.children = [];
      for (const child of actualChildren) {
        displayNode.children.push(await this.buildDisplayNode(child));
      }
    } else if (node.has_children) {
      displayNode.children = true;
    } else {
      displayNode.children = false;
    }

    return displayNode;
  }

  async getNodes(parent_id) {
    if (parent_id === '#') {
      const rootNodes = [];
      for (const node of this.nodes.values()) {
        if (!node.parent()) {
          rootNodes.push(await this.buildDisplayNode(node));
        }
      }
      return rootNodes;
    }

    const parentNode = this.getNode(parent_id);
    if (!parentNode) {
      return [];
    }

    const displayChildren = [];
    for (const child of parentNode.children()) {
      displayChildren.push(await this.buildDisplayNode(child));
    }
    return displayChildren;
  }

  async getProperties(id) {
    return await dbManager.getData('attributes', id);
  }
}
