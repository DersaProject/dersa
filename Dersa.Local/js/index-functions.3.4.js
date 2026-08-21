// Переключение вкладок
function switchTab(tabId) {
	
  // Скрываем все вкладки
  document.querySelectorAll('.tab-content').forEach(tab => {
    tab.classList.remove('active');
  });

  // Активируем выбранную вкладку
  document.getElementById(tabId).classList.add('active');

  // Обновляем стили кнопок вкладок
  document.querySelectorAll('.tab-button').forEach(button => {
    button.classList.remove('active');
  });
  event.target.classList.add('active');
}

// Получение информации об узле
async function GetNodeInfo(node_id, nodeIsDiagram) {
//		let url = `/Node/Description?id=${node_id}`; 
//		if (nodeIsDiagram)
//			url += '&attr_name=DiagramXml';
	
//	try{
//		const response = await fetch(url);
//		if (response.ok) {
//			return await response.text();
//		}
//	} catch (error) {
//			console.warn('Загрузка данных не удалась:', error);
//	}
	if (nodeIsDiagram){
		const diagramData = await dTree.getDiagramData(node_id);
		return diagramData.xml;
	}
	return 'id = ' + node_id;
};


// Загрузка информации об узле в первую вкладку
async function loadNodeInfo(node_id) {
	const nodeIsDiagram = node_id[0]=="D";
	const state = diagramManager.getEditingState();
	if(state.isEdited)
		return;
	const nodeInfoDiv = document.getElementById('node-info-block');
	const diagInfoDiv = document.getElementById('diagram-block');

	try {
		const info = await GetNodeInfo(node_id, nodeIsDiagram);

		if(nodeIsDiagram)
		{
			nodeInfoDiv.style.display = 'none';
			diagInfoDiv.style.display = 'block';
			console.log('loading Diagram');
			diagramManager.loadFromXml(info, node_id);
			console.log('Diagram loaded');

			//graph.getModel().beginUpdate();
			//try {
			//	const doc = mxUtils.parseXml(info);
			//	const codec = new mxCodec(doc);
			//	codec.decode(doc.documentElement, graph.getModel());	} finally {
			//	graph.getModel().endUpdate();
			//}
		}
		else{
			nodeInfoDiv.style.display = 'block';
			diagInfoDiv.style.display = 'none';
			nodeInfoDiv.innerHTML = info;
		}
	} catch (error) {
		nodeInfoDiv.textContent = `Ошибка при загрузке: ${error.message}`;
	}
}

// Получение свойств узла
        async function GetNodeProperties(node_id) {
//            try {
//                // Попытка получить данные с сервера
//                const response = await fetch(`/Node/PropertiesForm/${node_id}`);
//                if (response.ok) {
//                    return await response.json();
//                }
//            } catch (error) {
//                console.warn('API недоступно, используем демо‑данные:', error);
//            }

            // Возвращаем [], если API недоступно
			if(node_id[0] === 'R'){//relation
				const relation = dTree.getRelation(node_id);
				return [{Name: "relation", Value: relation.relation_id},{Name: "stereotype", Value: relation.stereotype},{Name: "src", Value: relation.aNodeId},{Name: "dst", Value: relation.bNodeId}];
			}
			else{
				const node = dTree.getNode(node_id);
				if(node_id[0] === 'D')//diagram
					return [{Name: "diagram", Value: node.id},{Name: "name", Value: node.name}];
        let attrs = [{Name: "entity", Value: node.id},{Name: "stereotype", Value: node.stereotype},{Name: "name", Value: node.name}];  
        const savedAttrs = node.properties();
        if(savedAttrs)
        {
          for (const key in savedAttrs) {
            attrs.push({Name: key, Value: savedAttrs[key]});
          }          
        }

				return attrs;
			}
			//return JSON.parse(await dbManager.getData('attributes', node_id));
        }


// Получение детального значения свойства
async function GetPropertyValue(node_id, property_name) {

  const response = await fetch(`/Node/PropertyForm?id=${node_id}&prop_name=${property_name}&prop_type=2`);   //GetPropertyValue
                if (response.ok) {
                    const resultObj = await response.json();
		    const property = resultObj.find(p => p.Name === property_name);
                    return property.Value;
                }
}

// Загрузка свойств узла в таблицу
async function loadNodeProperties(node_id) {
  const tableBody = document.querySelector('#properties-table tbody');
  tableBody.innerHTML = ''; // Очищаем предыдущее содержимое
  const propertyValueDiv = document.getElementById('property-value');
  propertyValueDiv.textContent = "Выберите свойство для просмотра";

  const properties = await GetNodeProperties(node_id);

  properties.forEach(property => {
    const row = document.createElement('tr');
    row.innerHTML = `
      <td>${property.Name}</td>
      <td>${needButton(property.Value) ? getButtonHtml(node_id, property.Name) : escapeHtml(property.Value)}</td>
    `;

    row.addEventListener('click', () => {
      // Сбрасываем подсветку предыдущих строк
      document.querySelectorAll('#properties-table tbody tr').forEach(r => {
        r.classList.remove('selected');
      });
      // Подсвечиваем текущую строку
      row.classList.add('selected');
      // Отображаем значение свойства
      displayPropertyValue(node_id, property.Name, true);
    });

    tableBody.appendChild(row);
  });
}

function needButton(text){
	return text && (text.length > 255 || (text.includes('<') && text.includes('>') && text.includes('/')));
}

function getButtonHtml(id, name){
	return `<button onclick="showEvent(event, '${name}', ${id})">html</button>`;
}

function showEvent(event, propertyName, nodeId){
  event.stopPropagation();
  displayPropertyValue(nodeId, propertyName, false);
}

// Отображение детального значения свойства
async function displayPropertyValue(node_id, property_name, doEscape) {
  const propertyValueDiv = document.getElementById('property-value');
  propertyValueDiv.textContent = "Загрузка...";

  try {
    const value = await GetPropertyValue(node_id, property_name);
    propertyValueDiv.innerHTML = doEscape? `<pre>${escapeHtml(value)}</pre>` : value;
  } catch (error) {
    propertyValueDiv.textContent = `Ошибка при загрузке: ${error.message}`;
  }
}

// Получение методов узла
async function GetNodeMethods(node_id) {
//            try {
//                // Попытка получить данные с сервера
//                const response = await fetch(`/Node/MethodsForm/${node_id}`);
//                if (response.ok) {
//                    return await response.json();
//                }
//            } catch (error) {
//                console.warn('API недоступно, используем демо‑данные:', error);
//            }

            // Возвращаем [], если API недоступно
            return [];
}

// Отображение результата вызова метода
var currentMethodResult = "";
async function displayMethodResult(node_id, method_name) {
  const methodResultDiv = document.getElementById('method-result');
  methodResultDiv.textContent = "Выполнение...";

  const response = await fetch(`/Node/ExecMethodForm?id=${node_id}&method_name=${method_name}`);   //GetMethodResult
                if (response.ok) {
                    //methodResultDiv.textContent = await response.text();
                    //methodResultDiv.innerHTML = '<pre>' +  await response.text() + '</pre>';
                    const resultObj = await response.json();
                    currentMethodResult = resultObj[0].Value;
                    methodResultDiv.innerHTML = '<pre>' + currentMethodResult + '</pre><br>--------<br><button onclick="navigator.clipboard.writeText(currentMethodResult)">Copy</button>';
                }

}

// Загрузка методов узла в таблицу
async function loadNodeMethods(node_id) {
  const methodResultDiv = document.getElementById('method-result');
  methodResultDiv.innerHTML = "";
  const tableBody = document.querySelector('#methods-table tbody');
  tableBody.innerHTML = ''; // Очищаем предыдущее содержимое

  const methods = await GetNodeMethods(node_id);

  methods.forEach(method => {
    const row = document.createElement('tr');
    row.innerHTML = `
      <td>${method.Name}</td>
      <td><button onclick="handleMethodClick('${node_id}', '${method.Name}')">=&gt;</button></td>
    `;

    row.addEventListener('click', () => {
      // Сбрасываем подсветку предыдущих строк
      document.querySelectorAll('#methods-table tbody tr').forEach(r => {
        r.classList.remove('selected');
      });
      // Подсвечиваем текущую строку
      row.classList.add('selected');
    });

    tableBody.appendChild(row);
  });
}

// Обработчик клика по кнопке метода
function handleMethodClick(node_id, method_name) {
  displayMethodResult(node_id, method_name);
}

async function saveDiagram(diagramId, xml){
	dTree.saveDiagramData(diagramId, xml);
}

async function getRelationInfo(){
	return { fromEntityId: 'L1_1', toEntityId: 'L3_1' } ;
}



let diagramManager;
// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', function() {
// Инициализируем первую вкладку — показываем блок с информацией
document.getElementById('node-info-block').style.display = 'block';
document.getElementById('diagram-block').style.display = 'none';
//const container = document.getElementById('diagram');
//diagramManager = new DiagramManager(container);

const diagramContainer = document.getElementById('diagramContainer');
const buttonsContainer = document.getElementById('buttonsContainer');

// Создаём экземпляр DiagramManager
diagramManager = new DiagramManager(diagramContainer, buttonsContainer, saveDiagram, getRelationInfo);  

});

let initialNodeId;

/**
     * Экранирует HTML-символы для безопасного вывода
     * @param {string} unsafe - текст, который нужно экранировать
     * @returns {string} экранированный текст
     */
    function escapeHtml(unsafe) {
	  if(!unsafe)
          return unsafe;		  
      return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
    }