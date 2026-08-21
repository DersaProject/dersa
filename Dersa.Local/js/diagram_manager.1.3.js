/**
 * Класс для управления диаграммами mxGraph
 * @param {HTMLElement} diagramContainer - контейнер для отображения диаграммы
 * @param {HTMLElement} buttonsContainer - контейнер для кнопок управления
 */
class DiagramManager {
constructor(diagramContainer, buttonsContainer, cbSaveMethod) {
    if (!diagramContainer || !buttonsContainer) {
      throw new Error('Both diagramContainer and buttonsContainer are required');
    }

    this.diagramContainer = diagramContainer;
    this.buttonsContainer = buttonsContainer;
    // Внешний метод сохранения, передаётся извне
    this.cbSaveMethod = cbSaveMethod;

    this.isEditing = false;
    this.savedXml = null;
    this.diagramId = null; 
    
    // Карта сущностей: entityId -> { x, y, width, height }
    this.entitiesMap = new Map();

    this.createButtons();
    this.initEditor();
  }

  /**
   * Инициализирует редактор mxGraph
   */
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

    // Контекстное меню для редактирования стиля
    graph.popupMenuHandler.factoryMethod = (menu, cell, evt) => {
      if (!this.isEditing || !cell) return;

      menu.clear(); // Очищаем стандартное меню
      
      menu.addItem('Редактировать стиль', null, () => {
        const currentStyle = graph.getCellStyle(cell);
        const newStyle = prompt('Введите новый стиль:', currentStyle);
        if (newStyle !== null && newStyle.trim() !== '') {
          graph.setCellStyle(newStyle.trim(), [cell]);
        }
      });

      menu.addSeparator();
      menu.addItem('Удалить', null, () => {
        graph.removeCells([cell]);
      });
    };

    // Обработчик клавиш (Delete)
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

  /**
   * Загружает диаграмму из XML
   * @param {string} xml - XML-описание диаграммы
   * @param {string} diagramId - идентификатор диаграммы
   */
  loadFromXml(xml, diagramId) {
    this.diagramId = diagramId;
    
    const doc = mxUtils.parseXml(xml);
    const node = doc.documentElement;
    const dec = new mxCodec(node);
    
    dec.decode(node, this.editor.graph.getModel());

    this.savedXml = xml;
    this.isEditing = false;
    
    // При загрузке тоже можно обновить карту сущностей (опционально)
    // this.extractEntitiesFromXml(xml); 

    this.updateButtonVisibility();
  }

  /**
   * Извлекает сущности из XML и заполняет this.entitiesMap
   * Формат ключа: entity ID (строка/число)
   * Формат значения: { x, y, width, height }
   * 
   * Ищем: <mxCell ...><Object entity="..." ... /><mxGeometry x="..." y="..." ... /></mxCell>
   */
  extractEntitiesFromXml(xml) {
    const doc = mxUtils.parseXml(xml);
    // Находим все узлы mxCell
    const cells = doc.getElementsByTagName('mxCell');

    for (let i = 0; i < cells.length; i++) {
      const cellNode = cells[i];
      
      // Ищем дочерний узел Object с атрибутом entity
      const objectNode = cellNode.querySelector('Object');
      if (!objectNode) continue;

      const entityId = objectNode.getAttribute('entity');
      if (!entityId) continue; // Пропускаем, если нет entity

      // Ищем дочерний узел mxGeometry
      const geometryNode = cellNode.querySelector('mxGeometry');
      if (!geometryNode) continue;

      const x = parseFloat(geometryNode.getAttribute('x')) || 0;
      const y = parseFloat(geometryNode.getAttribute('y')) || 0;
      const width = parseFloat(geometryNode.getAttribute('width')) || 0;
      const height = parseFloat(geometryNode.getAttribute('height')) || 0;

      // Сохраняем в Map. Если сущность встречается несколько раз, перезапишется последней
      this.entitiesMap.set(entityId, {
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

    // 1. Получаем текущий XML диаграммы
    const currentXml = this.getXml();
    
    // 2. Извлекаем сущности из текущего XML и обновляем entitiesMap
    this.extractEntitiesFromXml(currentXml);

    // 3. Сохраняем XML как последнее сохранённое состояние (для cancel)
    this.savedXml = currentXml;

    const self = this;

    // 4. Вызываем внешний метод сохранения, переданный в конструкторе
    this.cbSaveMethod(this.diagramId, currentXml)
      .then(() => {
        // Успешное сохранение: выходим из режима редактирования
        self.isEditing = false;
        const graph = self.editor.graph;
        graph.setEnabled(false);      // Отключаем редактирование
        graph.setDropEnabled(false);  // Запрещаем drag and drop
        self.updateButtonVisibility();
      })
      .catch(error => {
        console.error('Ошибка сохранения:', error);
        // При ошибке можно либо оставить в режиме редактирования, либо сбросить — на ваше усмотрение.
        // Сейчас оставляем в режиме редактирования, чтобы пользователь мог попробовать снова или отменить.
      });
  }
  cancel() {
    if (!this.isEditing) return;

    this.isEditing = false;

    if (this.savedXml) {
      // При отмене восстанавливаем XML, но карту сущностей лучше тоже восстановить из сохраненного состояния
      // чтобы она соответствовала тому, что реально лежит на сервере/в savedXml
      this.loadFromXml(this.savedXml, this.diagramId);
      // Перестраиваем карту на основе восстановленного XML
      this.extractEntitiesFromXml(this.savedXml);
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

  createElementFromDrop(type, x, y, label, id) {
    const graph = this.editor.graph;
    let style = '';
    let width = 120;
    let height = 25;

    const parent = graph.getDefaultParent();
    graph.getModel().beginUpdate();

    try {
      if (id > 0) {
        var v1 = graph.insertVertex(parent, null, { entity: id, label: label }, x, y, width, height, style);
      } else {
        var rel = -id;
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

  getXml() {
    const graph = this.editor.graph;
    const encoder = new mxCodec();
    const node = encoder.encode(graph.getModel());
    return mxUtils.getXml(node);
  }

  getEditingState() {
    return { isEdited: this.isEditing };
  }

  /**
   * Возвращает карту сущностей
   * @returns {Map} Map<entityId, {x, y, width, height}>
   */
  getEntities() {
    // Возвращаем копию Map или саму Map, чтобы внешний код мог итерироваться
    return this.entitiesMap;
  }
}

// Экспорт для модулей
if (typeof module !== 'undefined' && module.exports) {
  module.exports = DiagramManager;
}
