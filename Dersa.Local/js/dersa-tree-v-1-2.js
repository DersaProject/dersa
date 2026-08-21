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
  constructor(dManager) {
    this.nodes = new Map();
	this.dManager = dManager;
  }

  getNode(node_id) {
    return this.nodes.get(node_id) || null;
  }


  async loadNodes(parent_id) {
    try {
      const childrenJSON = await this.dManager.getData('entities', parent_id);
      const childrenData = JSON.parse(childrenJSON);
	  console.log(childrenData);
	  const relations = await this.dManager.findByKeyValue('relations', 'a', 'L1_1');
	  console.log(relations);
	  

      const parentNode = parent_id === '#' ? null : this.getNode(parent_id);

      for (const nodeData of childrenData) {
        // Создаём узел
        const node = new DersaNode(
          nodeData.id,
          nodeData.stereotype,
          nodeData.name,
          nodeData.has_children || false
        );
        this.nodes.set(node.id, node);

        // Устанавливаем связь с родителем
        if (parentNode) {
          parentNode.addChild(node);
        }

        // Если есть дочерние узлы в данных, рекурсивно загружаем их
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
      }

      return childrenData.map(node => node.id);
    } catch (error) {
      console.error(`Ошибка при загрузке узлов для parent_id ${parent_id}:`, error);
      throw error;
    }
  }

  // Вспомогательный метод для рекурсивной загрузки поддерева
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

      // Если у узла есть свои дети, продолжаем рекурсию
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
      // Возвращаем всё дерево — все корневые узлы
      const rootNodes = [];
      for (const node of this.nodes.values()) {
        if (!node.parent()) {
          rootNodes.push(await this.buildDisplayNode(node));
        }
      }
      return rootNodes;
    }

    // Возвращаем потомков указанного узла
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
