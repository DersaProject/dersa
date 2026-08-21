/**
 * Класс для управления диаграммами mxGraph (версия с уникальностью entity_id)
 */
class DiagramManager {
  /**
   * @param {HTMLElement} diagramContainer - контейнер для диаграммы
   * @param {HTMLElement} buttonsContainer - контейнер для кнопок
   * @param {Function} cbSaveMethod - внешний метод сохранения (возвращает Promise)
   */
  constructor(diagramContainer, buttonsContainer, cbSaveMethod) {
    if (!diagramContainer || !buttonsContainer) {
      throw new Error('Both diagramContainer and buttonsContainer are required');
    }

    this.diagramContainer = diagramContainer;
    this.buttonsContainer = buttonsContainer;
    this.cbSaveMethod = cbSaveMethod;

    this.isEditing = false;
    this.savedXml = null;
    this.diagramId = null; 
    
    // Map<entityId (string), { cellId: string, x: number, y: number, width: number, height: number }>
    // Хранит актуальное состояние сущностей на диаграмме
    this.entitiesMap = new Map();

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
    graph.setMultigraph(false);

    // Настройка отображения меток
    graph.convertValueToString = function(cell) {
      const value = cell.getValue();
      if (value && value.label) {
        return value.label;
      }
      return mxGraph.prototype.convertValueToString.apply(this, arguments);
    };

    graph.setEnabled(false);
    graph.setDropEnabled(false);

    // Контекстное меню
    graph.popupMenuHandler.factoryMethod = (menu, cell, evt) => {
      // ВАЖНО: menu.clear() — НЕЛЬЗЯ вызывать, такого метода нет в mxPopupMenu
      
      if (!this.isEditing || !cell) {
        // Если не режим редактирования или клик не по ячейке — можно вообще ничего не делать
        return;
      }

      // Вместо clear() мы просто не добавляем стандартные пункты,
      // а добавляем только те, что нам нужны.
      // В factoryMethod меню уже «пустое» для этой ячейки — это стандартное поведение mxGraph.

      menu.addItem('Редактировать стиль', null, () => {
        const currentStyle = graph.getCellStyle(cell);
        const newStyle = prompt('Введите стиль:', currentStyle);
        if (newStyle !== null && newStyle.trim() !== '') {
          graph.setCellStyle(newStyle.trim(), [cell]);
        }
      });

      menu.addSeparator();
      menu.addItem('Удалить', null, () => {
        graph.removeCells([cell]);
      });
    };

    // Обработчик Delete
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
    
    // Перестраиваем карту сущностей на основе загруженного XML
    this.extractEntitiesFromXml(xml);

    this.updateButtonVisibility();
  }

  /**
   * Извлекает сущности из XML.
   * Логика: один entity_id -> одна запись в Map.
   * Если в XML несколько ячеек имеют один entity_id, в Map попадёт последняя (по порядку обхода).
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
      if (!entityId) continue; 

      const geometryNode = cellNode.querySelector('mxGeometry');
      if (!geometryNode) continue;

      const x = parseFloat(geometryNode.getAttribute('x')) || 0;
      const y = parseFloat(geometryNode.getAttribute('y')) || 0;
      const width = parseFloat(geometryNode.getAttribute('width')) || 0;
      const height = parseFloat(geometryNode.getAttribute('height')) || 0;

      // Записываем/перезаписываем запись для этого entityId.
      // Теперь мы знаем cellId этой ячейки, чтобы двигать её при повторном дропе.
      this.entitiesMap.set(entityId, {
        cellId: cellId,
        x: x,
        y: y,
        width: width,
        height: height
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
    
    // Обновляем карту сущностей перед сохранением, чтобы она соответствовала тому, что уйдёт на сервер
    this.extractEntitiesFromXml(currentXml);

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
      // Восстанавливаем XML
      this.loadFromXml(this.savedXml, this.diagramId);
      // Карта сущностей уже перестроена внутри loadFromXml через extractEntitiesFromXml
    } else {
      const graph = this.editor.graph;
      graph.getModel().clear();
      this.entitiesMap.clear();
    }

    const graph = this.editor.graph;
    graph.setEnabled(false);
    graph.setDropEnabled(false);
    this.updateButtonVisibility();
  }

  /**
   * Создаёт элемент или двигает существующий, если entity уже есть на диаграмме.
   */
    /**
   * Создаёт элемент или двигает существующий, если entity уже есть на диаграмме.
   * ID теперь строковые: сущности — L..., отношения — R...
   */
  createElementFromDrop(type, x, y, label, id) {
    const graph = this.editor.graph;
    let style = '';
    let width = 120;
    let height = 25;

    const parent = graph.getDefaultParent();
    graph.getModel().beginUpdate();

    try {
      // Нормализуем id к строке (на случай, если где-то передаётся число)
      const idStr = String(id);

      if (idStr.startsWith('L')) {
        // --- СУЩНОСТЬ (Entity) ---
        const entityId = idStr; // ключ для Map — это "L..."
        
        const existingEntity = this.entitiesMap.get(entityId);

        if (existingEntity) {
          // СУЩНОСТЬ УЖЕ ЕСТЬ: двигаем существующую ячейку
          const existingCell = graph.getModel().getCell(existingEntity.cellId);
          
          if (existingCell) {
            // Сдвигаем ячейку в точку дропа
            graph.moveCells([existingCell], x - existingEntity.x, y - existingEntity.y, false, null, true);
            
            // Обновляем координаты в карте
            this.entitiesMap.set(entityId, {
              ...existingEntity,
              x: x,
              y: y
            });
            
            console.log(`Сущность ${entityId} перемещена в (${x}, ${y})`);
          } else {
            // Ячейка потерялась (редкий кейс), создаём новую
            const v1 = graph.insertVertex(parent, null, { entity: entityId, label: label }, x, y, width, height, style);
            this.updateEntitiesMapAfterCreate(v1, entityId, x, y, width, height);
          }
        } else {
          // СУЩНОСТИ НЕТ: создаём новую вершину
          const v1 = graph.insertVertex(parent, null, { entity: entityId, label: label }, x, y, width, height, style);
          this.updateEntitiesMapAfterCreate(v1, entityId, x, y, width, height);
        }

      } else if (idStr.startsWith('R')) {
        // --- ОТНОШЕНИЕ (Relation) ---
        // Отношения не имеют уникальности по ID в том же смысле,
        // поэтому создаём как раньше (через XHR запрос к API)
        var rel = idStr; // оставляем как строку "R..."
        var xhr = new XMLHttpRequest();
        console.log(this.diagramId, rel);
        let args = "diagram=" + this.diagramId.replace('D_', '') + "&relation=" + rel;
        xhr.open('GET', "Diagram/RelationInfo?" + args, false);
        xhr.send();
        var attrs = JSON.parse(xhr.responseText);
        if (attrs.length) {
          attrs.forEach(function(item) {
            var e1 = graph.insertEdge(null, null, { relation: rel, label: item.label }, item.src, item.dst, item.style);
          });
        }
      } else {
        // --- ЗАПАСНОЙ ВАРИАНТ (если формат ID непонятен) ---
        // Обрабатываем как отношение, чтобы не ломать старую логику
        var rel = -Number(idStr) || idStr;
        var xhr = new XMLHttpRequest();
        console.log(this.diagramId, rel);
        let args = "diagram=" + this.diagramId.replace('D_', '') + "&relation=" + rel;
        xhr.open('GET', "Diagram/RelationInfo?" + args, false);
        xhr.send();
        var attrs = JSON.parse(xhr.responseText);
        if (attrs.length) {
          attrs.forEach(function(item) {
            var e1 = graph.insertEdge(null, null, { relation: rel, label: item.label }, item.src, item.dst, item.style);
          });
        }
      }
    } finally {
      graph.getModel().endUpdate();
    }
  }

  /**
   * Вспомогательный метод для добавления новой сущности в Map после создания ячейки.
   * Вызывается только при создании новой вершины (не при перемещении).
   */
  updateEntitiesMapAfterCreate(cell, entityId, x, y, w, h) {
    // Получаем реальный ID ячейки из mxCell (он может отличаться от того, что мы думали)
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
}

if (typeof module !== 'undefined' && module.exports) {
  module.exports = DiagramManager;
}
