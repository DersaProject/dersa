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
}

class DersaTree {
  /**
   * @param {Object} [dbManager] - Менеджер базы данных (может быть null/undefined на старте)
   */
  constructor(dbManager) {
    this.dbManager = dbManager || null;
    this.nodes = new Map();
    this.relations = null;
  }

  getNode(node_id) {
    return this.nodes.get(node_id) || null;
  }

  async getDiagramData(node_id){
      const diagramData = await this.dbManager.getData('diagrams', node_id);
	  return diagramData;
  }

  async saveDiagramData(diagram_id, xml){
    await this.dbManager.saveData('diagrams', diagram_id, {xml: xml});
	console.log('diagram saved');
  }

  /**
   * Возвращает массив объектов вида {relation_id, stereotype, aNodeId, bNodeId}
   */
  async loadRelations() {
    // Здесь будет реальная логика с this.dbManager
	const records = await this.dbManager.getAllData('relations');
	const result = records.map(item => {
		if (typeof item.data === 'object' && item.data !== null) {
		  item.data["relation_id"] = 'R' + item.id;	
		  return item.data;
		}
	// Если вдруг попадётся не объект — возвращаем как есть (или можно выбросить ошибку)
		return item.data;
	});
	return result;
  }

  /**
   * Получить связь по relation_id
   * @param {any} relation_id - идентификатор связи
   * @returns {Object|null} объект связи или null, если не найден
   */
  getRelation(relation_id) {
    const relation = this.relations.find(r => r.relation_id === relation_id);
    return relation || null;
  }

  /**
   * Получить все исходящие связи для узла (где он — aNodeId)
   * Возвращает массив {relation_id, stereotype, bNodeId}
   */
  getARelations(node_id) {
    return this.relations
      .filter(r => r.aNodeId === node_id)
      .map(r => ({
        relation_id: r.relation_id,
        stereotype: r.stereotype,
        bNodeId: r.bNodeId
      }));
  }

  /**
   * Получить все входящие связи для узла (где он — bNodeId)
   * Возвращает массив {relation_id, aNodeId} (без stereotype)
   */
  getBRelations(node_id) {
    return this.relations
      .filter(r => r.bNodeId === node_id)
      .map(r => ({
        relation_id: r.relation_id,
        aNodeId: r.aNodeId
      }));
  }

  /**
   * Асинхронный метод получения JSON-данных о дочерних узлах
   * Для parent_id === '#' возвращает корневой узел, иначе пустой массив
   */
  async getChildrenJSON(parent_id) {
    if (parent_id === '#') {
      return JSON.stringify([
        { id: 1, stereotype: 'Package', name: 'root', has_children: false }
      ]);
    }
    return '[]';
  }

  /**
   * Асинхронная загрузка узлов (и их поддеревьев, если есть children в данных)
   * parent_id === '#' означает загрузку корневых узлов
   */
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

        // Если в данных есть children — рекурсивно строим поддерево
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

            // Рекурсивно обрабатываем внуков, если они есть
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

  /**
   * Вспомогательный метод для рекурсивной загрузки вложенных children
   */
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

  /**
   * Построение узла в формате для отображения (с рекурсией по детям)
   */
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
      // Есть потенциальные дети, но ещё не загружены
      displayNode.children = true;
    } else {
      // Детей нет и не ожидается
      displayNode.children = false;
    }

    return displayNode;
  }

  /**
   * Получение узлов в формате для отображения
   * Если parent_id === '#', возвращается всё дерево (все корневые узлы)
   * Иначе — только прямые потомки узла parent_id
   */
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
