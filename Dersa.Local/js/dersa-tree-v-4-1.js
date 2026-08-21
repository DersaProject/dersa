class DersaNode {
  constructor(id, stereotype, name, has_children = false) {
    this.id = id;
    this.stereotype = stereotype;
    this.name = name;
    this.has_children = has_children;
    this._parent = null;
    this._children = [];
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

  addChild(childNode) {
    this._children.push(childNode);
    childNode.setParent(this);
  }

  /**
   * Сгенерировать ID для нового дочернего узла
   * @param {DersaTree} tree - экземпляр дерева
   * @param {string} childStereotype - стереотип (тип) нового узла
   * @returns {string} Новый ID
   */
  generateId(tree, childStereotype) {
    const parentId = this.id;
    return tree.generateNewNodeId(parentId, childStereotype);
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

  /**
   * Получить максимальный номер для L-узлов (тип 2: каталог, содержащий сущности).
   * Теперь тип 2 имеет стереотип 'Package'.
   * ID имеют вид: L1, L2, L3, ...
   */
  getMaxLNumber() {
    let maxNum = 0;

    for (const node of this.nodes.values()) {
      // Тип 2 — это Package
      if (node.stereotype === 'Package') {
        const match = node.id.match(/^L(\d+)$/);
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
   * - Тип 2 (Package): L1, L2, L3, ...
   * - Типы 1 (SuperPackage) и 3 (любые другие): <parentId>_1, <parentId>_2, ...
   */
  generateNewNodeId(parentId, stereotype) {
    // Тип 2 теперь — 'Package'
    const type2Stereotype = 'Package';

    if (stereotype === type2Stereotype) {
      // Для каталога, содержащего сущности: L<номер>
      const nextNum = this.getMaxLNumber() + 1;
      return `L${nextNum}`;
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

  async saveDiagramData(diagram_id, xml){
    await this.dbManager.saveData('diagrams', diagram_id, {xml: xml});
    console.log('diagram saved');
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
    if(!Array.isArray(this.relations))
      this.relations = await this.loadRelations();
    try {
      const childrenData = await this.dbManager.getData('entities', parent_id);

      const parentNode = parent_id === '#' ? null : this.getNode(parent_id);

      for (const nodeData of childrenData) {
        const node = new DersaNode(
          nodeData.id,
          nodeData.stereotype,
          nodeData.name,
          nodeData.has_children || false
        );
        this.nodes.set(node.id, node);

        if (parentNode) {
          parentNode.addChild(node);
        }

        if (Array.isArray(nodeData.children) && nodeData.children.length > 0) {
          for (const childData of nodeData.children) {
            const childNode = new DersaNode(
              childData.id,
              childData.stereotype,
              childData.name,
              childData.has_children || false
            );
            this.nodes.set(childNode.id, childNode);
            node.addChild(childNode);

            if (Array.isArray(childData.children) && childData.children.length > 0) {
              await this._loadSubtree(childNode, childData.children);
            }
          }
        }

        const aRelations = this.getARelations(nodeData.id);
        if(Array.isArray(aRelations) && aRelations.length > 0){
          for (const R of aRelations){
            node.addChild(new DersaNode(R.relation_id, R.stereotype, R.bNodeId, false));
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
        childData.has_children || false
      );
      this.nodes.set(childNode.id, childNode);
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
}
