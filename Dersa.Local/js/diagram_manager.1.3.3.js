class DiagramManager {
  /**
   * @param {HTMLElement} diagramContainer
   * @param {HTMLElement} buttonsContainer
   * @param {Function} cbSaveMethod - сохранение диаграммы (id, xml) -> Promise
   * @param {Function} cbGetRelationInfo - получение данных отношения (relationId) -> Promise<{ fromEntityId: string, toEntityId: string } | null>
   */
  constructor(diagramContainer, buttonsContainer, cbSaveMethod, cbGetRelationInfo) {
    if (!diagramContainer || !buttonsContainer) {
      throw new Error('Both diagramContainer and buttonsContainer are required');
    }

    this.diagramContainer = diagramContainer;
    this.buttonsContainer = buttonsContainer;
    this.cbSaveMethod = cbSaveMethod;
    this.cbGetRelationInfo = cbGetRelationInfo;

    this.isEditing = false;
    this.savedXml = null;
    this.diagramId = null; 
    
    // Map<entityId (L...), { cellId, x, y, width, height }>
    this.entitiesMap = new Map();
    
    // Map<relationId (R...), { cellId, fromEntityId, toEntityId }>
    // Хранит только те связи, которые реально есть на диаграмме
    this.relationsMap = new Map();

    this.createButtons();
    this.initEditor();
  }

initEditor() {
  const config = {};
  this.editor = new mxEditor(config);
  this.editor.setGraphContainer(this.diagramContainer);

  const graph = this.editor.graph;

  graph.setConnectable(true);
  new mxConnectionHandler(graph);
  graph.setAllowDanglingEdges(false);
  
  // ВАЖНО: оставляем true, чтобы можно было кидать два отношения на одну пару сущностей
  // и они не схлопывались в одну линию
  graph.setMultigraph(true); 

  graph.convertValueToString = function(cell) {
    const value = cell.getValue();
    if (value && value.label) {
      return value.label;
    }
    return mxGraph.prototype.convertValueToString.apply(this, arguments);
  };

  graph.setEnabled(false);
  graph.setDropEnabled(false);

  // Контекстное меню (оставляем как у тебя)
  graph.popupMenuHandler.factoryMethod = (menu, cell, evt) => {
    if (!this.isEditing || !cell) return;

    menu.addItem('Редактировать стиль', null, () => {
      const currentStyle = graph.getCellStyle(cell);
      console.log('Текущий стиль:', currentStyle);
      // Подсказка про elbowEdgeStyle, чтобы пользователь знал, что писать
      const newStyle = prompt('Введите стиль (например, edgeStyle=elbowEdgeStyle;html=1):', currentStyle || 'html=1');
      if (newStyle !== null && newStyle.trim() !== '') {
        graph.setCellStyle(newStyle.trim(), [cell]);
      }
    });

    menu.addSeparator();
    menu.addItem('Удалить', null, () => {
      graph.removeCells([cell]);
    });
  };

  this.keyHandler = new mxKeyHandler(graph);
  this.keyHandler.bindKey(46, () => {
    if (this.isEditing && graph.isEnabled()) {
      graph.removeCells();
    }
  });

  graph.container.setAttribute('tabindex', '0');
  graph.container.addEventListener('click', () => {
    graph.container.focus();
  });
}

  createButtons() {
    this.editButton = document.createElement('button');
    this.editButton.textContent = 'Edit';
    this.editButton.onclick = () => this.enterEditMode();
    this.buttonsContainer.appendChild(this.editButton);

    this.saveButton = document.createElement('button');
    this.saveButton.textContent = 'Save';
    this.saveButton.onclick = () => this.save();
    this.buttonsContainer.appendChild(this.saveButton);

    this.cancelButton = document.createElement('button');
    this.cancelButton.textContent = 'Cancel';
    this.cancelButton.onclick = () => this.cancel();
    this.buttonsContainer.appendChild(this.cancelButton);

    this.updateButtonVisibility();
  }

  updateButtonVisibility() {
    if (this.isEditing) {
      this.editButton.style.display = 'none';
      this.saveButton.style.display = 'block';
      this.cancelButton.style.display = 'block';
    } else {
      this.editButton.style.display = 'block';
      this.saveButton.style.display = 'none';
      this.cancelButton.style.display = 'none';
    }
  }

  loadFromXml(xml, diagramId) {
    this.diagramId = diagramId;
    
    const doc = mxUtils.parseXml(xml);
    const node = doc.documentElement;
    const dec = new mxCodec(node);
    
    dec.decode(node, this.editor.graph.getModel());

    this.savedXml = xml;
    this.isEditing = false;
    
    // Перестраиваем оба мэпа на основе XML
    this.extractEntitiesFromXml(xml);
    this.extractRelationsFromXml(xml);

    this.updateButtonVisibility();
  }

  /**
   * Извлекает сущности из XML.
   */
  extractEntitiesFromXml(xml) {
    const doc = mxUtils.parseXml(xml);
    const cells = doc.getElementsByTagName('mxCell');

    for (let i = 0; i < cells.length; i++) {
      const cellNode = cells[i];
      
      const cellId = cellNode.getAttribute('id');
      if (!cellId) continue;

      const objectNode = cellNode.querySelector('Object');
      if (!objectNode) continue;

      const entityId = objectNode.getAttribute('entity');
      if (!entityId || !entityId.startsWith('L')) continue; 

      const geometryNode = cellNode.querySelector('mxGeometry');
      if (!geometryNode) continue;

      const x = parseFloat(geometryNode.getAttribute('x')) || 0;
      const y = parseFloat(geometryNode.getAttribute('y')) || 0;
      const width = parseFloat(geometryNode.getAttribute('width')) || 0;
      const height = parseFloat(geometryNode.getAttribute('height')) || 0;

      this.entitiesMap.set(entityId, {
        cellId: cellId,
        x: x,
        y: y,
        width: width,
        height: height
      });
    }
  }

  /**
   * Извлекает отношения из XML.
   * Ищем рёбра (edges), у которых в Object есть relation="R..."
   * И сохраняем: relationId -> { cellId, fromEntityId?, toEntityId? }
   * Если from/to не указаны в XML — можно оставить null и восстановить при необходимости.
   */
  extractRelationsFromXml(xml) {
    this.relationsMap = new Map();

    const doc = mxUtils.parseXml(xml);
    const cells = doc.getElementsByTagName('mxCell');

    for (let i = 0; i < cells.length; i++) {
      const cellNode = cells[i];
      
      // Пропускаем вершины (vertex="1") — нам нужны только рёбра
      if (cellNode.getAttribute('vertex') === '1') continue;

      const cellId = cellNode.getAttribute('id');
      if (!cellId) continue;

      const objectNode = cellNode.querySelector('Object');
      if (!objectNode) continue;

      const relationId = objectNode.getAttribute('relation');
      if (!relationId || !relationId.startsWith('R')) continue;

      // Пытаемся вытащить from/to, если они есть в Object (зависит от того, как ты их туда пишешь)
      const fromEntityId = objectNode.getAttribute('fromEntity');
      const toEntityId   = objectNode.getAttribute('toEntity');

      this.relationsMap.set(relationId, {
        cellId: cellId,
        fromEntityId: fromEntityId || null,
        toEntityId: toEntityId || null
      });
    }
  }

  enterEditMode() {
    this.isEditing = true;
    const graph = this.editor.graph;
    graph.setEnabled(true);
    graph.setDropEnabled(true);
    this.updateButtonVisibility();
  }

  save() {
    if (!this.isEditing) return;

    const currentXml = this.getXml();
    
    // Обновляем оба мэпа перед сохранением
    this.extractEntitiesFromXml(currentXml);
    this.extractRelationsFromXml(currentXml);

    this.savedXml = currentXml;

    const self = this;

    this.cbSaveMethod(this.diagramId, currentXml)
      .then(() => {
        self.isEditing = false;
        const graph = self.editor.graph;
        graph.setEnabled(false);
        graph.setDropEnabled(false);
        self.updateButtonVisibility();
      })
      .catch(error => {
        console.error('Ошибка сохранения:', error);
      });
  }

  cancel() {
    if (!this.isEditing) return;

    this.isEditing = false;

    if (this.savedXml) {
      this.loadFromXml(this.savedXml, this.diagramId);
      // Оба мэпа уже перестроены внутри loadFromXml
    } else {
      const graph = this.editor.graph;
      graph.getModel().clear();
      this.entitiesMap.clear();
      this.relationsMap.clear();
    }

    const graph = this.editor.graph;
    graph.setEnabled(false);
    graph.setDropEnabled(false);
    this.updateButtonVisibility();
  }

   createElementFromDrop(type, x, y, label, id) {
    const graph = this.editor.graph;
    const idStr = String(id);

    // Для сущностей (P...) логика остаётся синхронной — тут всё ок
    if (idStr.startsWith('P')) {
      this._handleEntityDrop(x, y, label, idStr);
      return;
    }

    // Для отношений (R...) — только подготовка, без асинхронности внутри beginUpdate
    if (idStr.startsWith('R')) {
      // Если отношение уже есть — вообще ничего не делаем
      if (this.relationsMap.has(idStr)) {
        console.log(`Отношение ${idStr} уже есть на диаграмме.`);
        return;
      }

      // Запускаем асинхронную обработку отдельно
      this._handleRelationDropAsync(idStr, x, y, label)
        .catch(err => console.error('Ошибка обработки отношения:', err));
      
      return;
    }
  }

  /**
   * Синхронная обработка дропа сущности (внутри транзакции)
   */
  /**
   * Синхронная обработка дропа сущности (внутри транзакции)
   */
  _handleEntityDrop(x, y, label, entityId) {
    const graph = this.editor.graph;
    const parent = graph.getDefaultParent();
    
    graph.getModel().beginUpdate();
    try {
      const existingEntity = this.entitiesMap.get(entityId);

      if (existingEntity) {
        const existingCell = graph.getModel().getCell(existingEntity.cellId);
        if (existingCell) {
          graph.moveCells([existingCell], x - existingEntity.x, y - existingEntity.y, false, null, true);
          this.entitiesMap.set(entityId, { ...existingEntity, x: x, y: y });
        } else {
          // Рассинхронизация: ячейка пропала, создаём заново
          const v1 = graph.insertVertex(parent, null, { entity: entityId, label: label }, x, y, 120, 25, '');
          this._updateEntitiesMapAfterCreate(v1, entityId, x, y, 120, 25);
        }
      } else {
        const v1 = graph.insertVertex(parent, null, { entity: entityId, label: label }, x, y, 120, 25, '');
        this._updateEntitiesMapAfterCreate(v1, entityId, x, y, 120, 25);
      }
    } finally {
      graph.getModel().endUpdate();
    }
  }

  /**
   * Вспомогательный метод для добавления новой сущности в Map после создания ячейки.
   * Теперь с префиксом _ как приватный метод.
   */
  _updateEntitiesMapAfterCreate(cell, entityId, x, y, w, h) {
    const realCellId = cell.getId();
    this.entitiesMap.set(entityId, {
      cellId: realCellId,
      x: x,
      y: y,
      width: w,
      height: h
    });
  }


  /**
   * Асинхронная обработка дропа отношения (запускает свою транзакцию)
   */
  /**
   * Асинхронная обработка дропа отношения (запускает свою транзакцию)
   */
  async _handleRelationDropAsync(relationId, x, y, label) {
    // Сначала запрашиваем данные
    let info;
    try {
      info = await this.cbGetRelationInfo(relationId);
    } catch (e) {
      console.error(`Не удалось получить данные для отношения ${relationId}`, e);
      return;
    }

    if (!info) {
      console.warn(`Нет данных для отношения ${relationId}`);
      return;
    }

    const { fromEntityId, toEntityId } = info;

    // Проверяем, есть ли обе сущности на диаграмме
    const fromCellData = this.entitiesMap.get(fromEntityId);
    const toCellData   = this.entitiesMap.get(toEntityId);

    if (!fromCellData || !toCellData) {
      console.warn(
        `Для отношения ${relationId} не найдены сущности: from=${fromEntityId}, to=${toEntityId}. ` +
        `Убедитесь, что обе сущности уже размещены на диаграмме.`
      );
      return;
    }

    const graph = this.editor.graph;
    const parent = graph.getDefaultParent();

    graph.getModel().beginUpdate();
    try {
      const edge = graph.insertEdge(
        parent,
        null,
        {
          relation: relationId,
          fromEntity: fromEntityId,
          toEntity: toEntityId,
          label: label || ''
        },
        fromCellData.cellId, // source cell ID
        toCellData.cellId    // target cell ID
      );

      this.relationsMap.set(relationId, {
        cellId: edge.getId(),
        fromEntityId: fromEntityId,
        toEntityId: toEntityId
      });

      console.log(`Создано отношение ${relationId} между ${fromEntityId} и ${toEntityId}`);
    } finally {
      graph.getModel().endUpdate();
    }
  }

  updateEntitiesMapAfterCreate(cell, entityId, x, y, w, h) {
    const realCellId = cell.getId();
    this.entitiesMap.set(entityId, {
      cellId: realCellId,
      x: x,
      y: y,
      width: w,
      height: h
    });
  }

  getXml() {
    const graph = this.editor.graph;
    const encoder = new mxCodec();
    const node = encoder.encode(graph.getModel());
    return mxUtils.getXml(node);
  }

  getEditingState() {
    return { isEdited: this.isEditing };
  }

  getEntities() {
    return this.entitiesMap;
  }

  getRelations() {
    return this.relationsMap;
  }
}

if (typeof module !== 'undefined' && module.exports) {
  module.exports = DiagramManager;
}
