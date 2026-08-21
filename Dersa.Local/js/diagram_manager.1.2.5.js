/**
 * Класс для управления диаграммами mxGraph
 * @param {HTMLElement} diagramContainer - контейнер для отображения диаграммы
 * @param {HTMLElement} buttonsContainer - контейнер для кнопок управления
 */
class DiagramManager {
  constructor(diagramContainer, buttonsContainer, saveMethod) {
    if (!diagramContainer || !buttonsContainer) {
      throw new Error('Both diagramContainer and buttonsContainer are required');
    }

    this.diagramContainer = diagramContainer;
    this.buttonsContainer = buttonsContainer;
	this.cbSaveMethod = saveMethod;
    this.isEditing = false;
    this.savedXml = null;
    this.diagramId = null; // Инициализируем поле для идентификатора диаграммы

    // Создаём кнопки
    this.createButtons();

    // Инициализируем редактор
    this.initEditor();
  }

  /**
   * Инициализирует редактор mxGraph
   */
/**
 * Инициализирует редактор mxGraph
 */
initEditor() {
  const config = {};
  this.editor = new mxEditor(config);
  this.editor.setGraphContainer(this.diagramContainer);

  const graph = this.editor.graph;

  // Настройки для связей
  graph.setConnectable(true);
  new mxConnectionHandler(graph);
  graph.setAllowDanglingEdges(false);
  graph.setMultigraph(false);

  // Настройка отображения меток ячеек (согласно ТЗ)
  graph.convertValueToString = function(cell) {
    const value = cell.getValue();
    if (value && value.label) {
      return value.label;
    }
    return mxGraph.prototype.convertValueToString.apply(this, arguments);
  };

  // Отключаем взаимодействие по умолчанию
  graph.setEnabled(false);
  graph.setDropEnabled(false);

  // Обработчик клавиш (Delete для удаления)
  this.keyHandler = new mxKeyHandler(graph);
  this.keyHandler.bindKey(46, () => {
    if (this.isEditing && graph.isEnabled()) {
      graph.removeCells();
    }
  });

  // Делаем контейнер фокусируемым
  graph.container.setAttribute('tabindex', '0');
  graph.container.addEventListener('click', () => {
    graph.container.focus();
  });

  // Отключаем стандартное контекстное меню
  graph.popupMenuHandler.enabled = false;

  // Создаём собственное контекстное меню
  graph.container.oncontextmenu = (evt) => {
    evt.preventDefault();

    if (!this.isEditing) return;

    const pt = mxUtils.convertPoint(graph.container, evt.clientX, evt.clientY);
    const cell = graph.getCellAt(pt.x, pt.y);

    if (cell) {
      // Создаём кастомное меню
      const customMenu = document.createElement('div');
      customMenu.style.position = 'absolute';
      customMenu.style.left = evt.clientX + 'px';
      customMenu.style.top = evt.clientY + 'px';
      customMenu.style.background = 'white';
      customMenu.style.border = '1px solid #ccc';
      customMenu.style.padding = '5px';
      customMenu.style.zIndex = '1000';

      // Пункт редактирования стиля
      const styleItem = document.createElement('div');
      styleItem.textContent = 'Редактировать стиль';
      styleItem.style.padding = '3px 10px';
      styleItem.style.cursor = 'pointer';
      styleItem.onclick = () => {
        const currentStyle = graph.getCellStyle(cell);
		console.log(currentStyle);
        const newStyle = prompt('Введите новый стиль:', 'fillColor=yellow');
		//const newStyle = 'fillColor=yellow;strokeColor=blue';
        if (newStyle) {
		console.log(newStyle);
          graph.setCellStyle(newStyle, [cell]);
        }
        document.body.removeChild(customMenu);
      };
      customMenu.appendChild(styleItem);

      document.body.appendChild(customMenu);

      // Убираем меню при клике вне его
      document.addEventListener('click', function handler() {
        document.body.removeChild(customMenu);
        document.removeEventListener('click', handler);
      }, { once: true });
    }
  };
}
  /**
   * Создаёт кнопки управления (Edit, Save, Cancel) и добавляет их в контейнер
   */
  createButtons() {
    // Кнопка Edit
    this.editButton = document.createElement('button');
    this.editButton.textContent = 'Edit';
    this.editButton.onclick = () => this.enterEditMode();
    this.buttonsContainer.appendChild(this.editButton);

    // Кнопка Save
    this.saveButton = document.createElement('button');
    this.saveButton.textContent = 'Save';
    this.saveButton.onclick = () => this.save();
    this.buttonsContainer.appendChild(this.saveButton);

    // Кнопка Cancel
    this.cancelButton = document.createElement('button');
    this.cancelButton.textContent = 'Cancel';
    this.cancelButton.onclick = () => this.cancel();
    this.buttonsContainer.appendChild(this.cancelButton);

    // Устанавливаем начальную видимость кнопок
    this.updateButtonVisibility();
  }

  /**
   * Обновляет видимость кнопок в зависимости от состояния редактирования
   */
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
    // Сохраняем идентификатор диаграммы
    this.diagramId = diagramId;

    const dec = new mxCodec(mxUtils.parseXml(xml).documentElement);
    const node = mxUtils.parseXml(xml).documentElement;
    dec.decode(node, this.editor.graph.getModel());

    // Сохраняем текущее состояние как последнее сохранённое
    this.savedXml = xml;
    this.isEditing = false;

    // Обновляем видимость кнопок
    this.updateButtonVisibility();
  }

  /**
   * Переводит диаграмму в режим редактирования
   */
  enterEditMode() {
    this.isEditing = true;
    const graph = this.editor.graph;
    graph.setEnabled(true); // Включаем редактирование
    graph.setDropEnabled(true); // Разрешаем drag and drop
    this.updateButtonVisibility();
  }

  /**
   * Сохраняет изменения и выходит из режима редактирования
   */
  save() {
    if (!this.isEditing) return;

    // Сохраняем текущее состояние как последнее сохранённое
    this.savedXml = this.getXml();
    const self = this;

    //fetch('diagram/Save', {
    //  method: 'POST',
    //  headers: {
    //    'Content-Type': 'application/json'
    //  },
    //  body: JSON.stringify({ id: this.diagramId, xml: this.savedXml })
    //})
	this.cbSaveMethod(this.diagramId, this.savedXml)//метод сохранения передается извне в конструкторе, за него отвечает другая часть системы
      .then(() => {
        self.isEditing = false;
        const graph = self.editor.graph;
        graph.setEnabled(false); // Отключаем редактирование
        graph.setDropEnabled(false); // Запрещаем drag and drop
        self.updateButtonVisibility();
      })
      .catch(error => console.error('Ошибка:', error));
  }

  /**
   * Отменяет изменения и выходит из режима редактирования
   */
  cancel() {
    if (!this.isEditing) return;

    this.isEditing = false;

    // Восстанавливаем предыдущее сохранённое состояние
    if (this.savedXml) {
      this.loadFromXml(this.savedXml, this.diagramId);
    } else {
      // Если нет сохранённого состояния, очищаем диаграмму
      const graph = this.editor.graph;
      graph.getModel().clear();
    }

    const graph = this.editor.graph;
    graph.setEnabled(false); // Отключаем редактирование
    graph.setDropEnabled(false); // Запрещаем drag and drop

    this.updateButtonVisibility();
  }

  /**
   * Создаёт элемент на диаграмме из перетащенного объекта
   * @param {string} type - тип элемента
   * @param {number} x - координата X
   * @param {number} y - координата Y
   * @param {string} label - метка элемента
   * @param {number} id - идентификатор элемента
   */
  createElementFromDrop(type, x, y, label, id) {
    const graph = this.editor.graph;
    let style = '';
    let width = 120;
    let height = 25;

    // Создаём новую ячейку
    const parent = graph.getDefaultParent();
    graph.getModel().beginUpdate();

    try {
      if (id[0] === 'L') {
        var v1 = graph.insertVertex(parent, null, { entity: id, label: label }, x, y, width, height, style);
		console.log('inserted ');
		console.log(v1);
      }/* else {
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
      }*/
    } finally {
      graph.getModel().endUpdate();
    }
  }

  /**
   * Возвращает текущее состояние диаграммы в формате XML
   * @returns {string} XML-описание диаграммы
   */
  getXml() {
    const graph = this.editor.graph;
    const encoder = new mxCodec();
    const node = encoder.encode(graph.getModel());
    return mxUtils.getXml(node);
  }

  /**
   * Возвращает состояние редактирования
   * @returns {{isEdited: boolean}} объект с флагом состояния редактирования
   */
  getEditingState() {
    return { isEdited: this.isEditing };
  }
}

// Экспортируем класс, если используется модульная система
if (typeof module !== 'undefined' && module.exports) {
  module.exports = DiagramManager;
}
	