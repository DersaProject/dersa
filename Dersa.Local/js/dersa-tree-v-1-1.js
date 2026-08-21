class DersaNode {
  constructor(id, stereotype, name) {
    this.id = id;
    this.stereotype = stereotype;
    this.name = name;
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
  constructor(rootNodesJSON) {
    this.nodes = new Map();
    this.buildTree(rootNodesJSON);
  }

  buildTree(nodesData, parentNode = null) {
    nodesData.forEach(nodeData => {
      const node = new DersaNode(nodeData.id, nodeData.stereotype, nodeData.name);
      this.nodes.set(node.id, node);

      if (parentNode) {
        parentNode.addChild(node);
      }

      const hasChildren = nodeData.has_children === true;
      const childrenData = nodeData.children;

      if (Array.isArray(childrenData) && childrenData.length > 0) {
        this.buildTree(childrenData, node);
      } else if (hasChildren) {
        // Откладываем загрузку дочерних узлов до вызова getChildrenJSON
      }
    });
  }

  async getChildrenJSON(node_id) {
    // Заглушка: возвращает пустой массив
    return '[]';
  }

  GetNode(node_id) {
    return this.nodes.get(node_id) || null;
  }

  async buildSubtree(node) {
    const nodeData = this.GetNode(node.id);
    if (!nodeData) return null;

    const hasChildren = nodeData.has_children === true;
    let childrenArray = nodeData.children || [];

    if (hasChildren && (!Array.isArray(childrenArray) || childrenArray.length === 0)) {
      try {
        const childrenJSON = await this.getChildrenJSON(node.id);
        childrenArray = JSON.parse(childrenJSON);
        this.buildTree(childrenArray, node);
      } catch (error) {
        console.error(`Ошибка при загрузке дочерних узлов для узла ${node.id}:`, error);
        childrenArray = [];
      }
    }

    const children = [];
    for (const child of node.children()) {
      children.push(await this.buildSubtree(child));
    }

    return {
      id: node.id,
      text: node.name,
      icon: node.stereotype,
      children: children.length > 0 ? children : false
    };
  }

  async getNodes() {
    const rootNodes = [];
    for (const node of this.nodes.values()) {
      if (!node.parent()) { // Корневые узлы не имеют родителя
        rootNodes.push(await this.buildSubtree(node));
      }
    }
    return rootNodes;
  }
}
