class DersaEntity{
    static get attributes() {
        return [
        ];
    }

    static get exposedMethods() {
        return [
        ];
    }

    constructor(node){
        this._node = node;
        if(!node)
            return;
        const properties = node.properties;
        if(properties) {
            properties.forEach(p => {
                this[p["Name"]] = p["Value"];
            });
        }
        this._children = [];
        this._aRelations = [];
        const nodeChildren = node.children();
        if(nodeChildren) {
            nodeChildren.forEach(ch => {
                const ent = ch.getDersaEntity();
                if(ent instanceof DersaRelation)
                    this._aRelations.push(ent);
                else
                    this._children.push(ent);
            });
        }
    }
    get id() {
        return this._node.id;
    }

    get name() {
        return this._node.name;
    }

    get stereotype() {
        return this._node.stereotype;
    }

    get parent() {
        const parentNode = this._node.parent();
        if(!parentNode)
            return null;
        return parentNode.getDersaEntity();
    }

    get children() {
        return this._children;
    }

    get aRelations() {
        return this._aRelations;
    }
}

class DersaRelation extends DersaEntity{
    constructor(node){
        super(node);
        //this.aNodeId = 
    }

    get A() {
        return this._node._tree.getNode(this._node._aNodeId);
    }

    get B() {
        return this._node._tree.getNode(this._node._bNodeId);
    }
}

class Package extends DersaEntity {

    static get attributes() {
        return [
            {Name: 'Description', Type: LongText}
        ];
    }

    getConfiguration(configName){
        const configs = this.children.filter(c => c instanceof Configuration);
        let c = configs.find(conf => conf.name == configName);
        return c;
    }

}

class Configuration extends DersaEntity {

    getTextSetting(constName){
        const consts = this.children.filter(c => c instanceof Const);
        let res = consts.find(c => c.name == constName);
        return res.TextValue;
        //return `instance of ${this.name} got attribute ${templateName}`;
    }

}

class Const extends DersaEntity {

    static get attributes() {
        return [
            {Name: 'Value', Type: ShortText},
            {Name: 'TextValue', Type: LongText}
        ];
    }
}

class Script extends DersaEntity {

    static get attributes() {
        return [
            {Name: 'Code', Type: LongText}
        ];
    }

    static get exposedMethods() {
        return [
            'Exec'
        ];
    }

    Exec(){
        if(this.Code) {
            var f = new Function(this.Code);
            const result = f.apply(this);
            return result;
        }
    }

}

class Entity extends DersaEntity {

    static get attributes() {
        return [
            {Name: 'Description', Type: LongText}
        ];
    }

    static get exposedMethods() {
        return [
            'GetAttributes'
        ];
    }

    GetAttributes() {
        /*
        let attrs = new Map();
        this.children.filter(c => c.stereotype == 'Attribute').forEach( a => {
            attrs.set(a.name, {name: a.name, PhisicalName: a.PhisicalName});
        });
        */
        let attrs = [];
        this.children.filter(c => c.stereotype == 'Attribute').forEach( a => {
            attrs.push({name: a.name, PhisicalName: a.PhisicalName});
        });
        console.log(attrs);
        return JSON.stringify(attrs);
    }
}

class Report extends DersaEntity {

    static get attributes() {
        return [
            {Name: 'Description', Type: LongText}
        ];
    }

    static get exposedMethods() {
        return [
        ];
    }
}

class FilterForm extends DersaEntity {
    static get exposedMethods() {
        return [
            'GenerateJson'
        ];
    }

    GenerateJson() {
    // ── 1. resultLayout: группировка элементов по строкам (Y / 25) ──

    const formControls = this.children.filter(c => c instanceof FormControl);

// Группируем по Math.floor(Y / 25)
    const groupMap = {};
    for (const c of formControls) {
        const key = Math.floor(c.Y / 25);
        if (!groupMap[key]) groupMap[key] = [];
        groupMap[key].push(c);
    }

const resultLayout = Object.keys(groupMap)
        .sort((a, b) => a - b)              // orderby g.Key
        .map(key => {
        const line = groupMap[key]
            .sort((a, b) => a.X - b.X)      // g.OrderBy(i => i.X)
            .map(i => ({
            Name: i.name,
            width: i.Width
            }));
        return { line };
        });

    // ── 2. controlsDependencies: зависимости каждого контрола ──
    const controlsDependencies = formControls.map(i => {
        const dependencies = {};
        const relations = i.aRelations.filter(r => r instanceof Relation);
        for (const r of relations) {
        const dName = r.B.name;
        dependencies[dName] = dName;
        }
        return {
        Name: i.name,
        dependencies
        };
    });

    // ── 3. Запросы по типам контролов ──
    const queryCombo = this.children
        .filter(cb => cb instanceof ComboBox)
        .map(cb => ({
        Name: cb.name,
        Description: cb.Caption,
        Type: 3,
        Required: false,
        Items: cb.Items
        }));

    // ListBox → Type 3, Multiple: true
    const queryMultiCombo = this.children
        .filter(lb => lb instanceof ListBox)
        .map(lb => ({
        Name: lb.name,
        Description: lb.Caption,
        Type: 3,
        Multiple: true,
        Required: false,
        Items: lb.Items
        }));

    const queryText = this.children
        .filter(c => c instanceof TextBox)
        .map(c => ({
        Name: c.name,
        Description: c.Caption,
        Type: 0,
        Required: false
        }));

    const queryCheck = this.children
        .filter(c => c instanceof CheckBox)
        .map(c => ({
        Name: c.name,
        Description: c.Caption,
        Type: 1,
        Required: false
        }));

    const queryDate = this.children
        .filter(c => c instanceof DateTimeBox)
        .map(c => ({
        Name: c.name,
        Description: c.Caption,
        Type: 2,
        Required: false
        }));

    // ── 4. Объединение + добавление Dependencies и ValueDataSource ──
    const filtersContent = []
        .concat(queryCombo, queryMultiCombo, queryText, queryCheck, queryDate)
        .map(cbd => {
        // Находим зависимости для текущего контрола по имени
        const depEntry = controlsDependencies.find(i => i.Name === cbd.Name);
        if (depEntry && Object.keys(depEntry.dependencies).length > 0) {
            cbd.Dependencies = depEntry.dependencies;
        }

        // Для Type === 3 (комбо) — строим ValueDataSource
        if (String(cbd.Type) === '3') {
            const cbValues = {};
            const stringValues = String(cbd.Items).split('\n');
            let key = 1;
            for (const val of stringValues) {
            cbValues[String(key++)] = val.trim();
            }
            cbd.ValueDataSource = {
            Type: 1,
            DataSetFields: cbValues
            };
        }

        return cbd;
        });

    // ── 5. Сериализация в итоговую строку ──
    const layoutJson = JSON.stringify(resultLayout, null, 2);
    const filtersJson = JSON.stringify(filtersContent, null, 2);

    return `const layoutData = ${layoutJson};\r\n\r\n\r\nconst globalFilterParams = ${filtersJson};`;
    }

    GetControls() {
        return this.children.filter(c => c instanceof FormControl);
    }
}

class Type extends DersaEntity {
}

class JSForm extends DersaEntity {

    static get attributes() {
        return [
            {Name: 'innerHTML', Type: LongText}
        ];
    }

    static get exposedMethods() {
        return [
            'Show'
        ];
    }

    Show(plainText) {
        let html = this.innerHTML;
        if(plainText){
            html = html.replaceAll('\n', '<br>'); 
        }
        let W = window.open('','newwin','width=600,height=400,status=1,menubar=1');
        W.document.open();
        W.document.write('<html><head><title>');
        W.document.write(this.Name);
        W.document.write('</title></head><body>');
        W.document.write(html);
        W.document.write('</body></html>');
        W.document.close();
        W.focus();
        return 'voila';
    }
}

class FormControl extends DersaEntity {
    static get attributes() {
        return [
            {Name: 'Caption', Type: ShortText},
            {Name: 'X', Type: ShortText},
            {Name: 'Y', Type: ShortText},
            {Name: 'Width', Type: ShortText},
            {Name: 'Height', Type: ShortText}
        ];
    }
}

class ComboBox extends FormControl {
static get attributes() {
    // Берём массив атрибутов от родителя
    const baseAttributes = super.attributes;

    return [
      ...baseAttributes,
      { Name: 'Items', Type: LongText }
    ];
  }
}

class ListBox extends FormControl {
static get attributes() {
    // Берём массив атрибутов от родителя
    const baseAttributes = super.attributes;

    return [
      ...baseAttributes,
      { Name: 'Items', Type: LongText }
    ];
  }
}

class TextBox extends FormControl {
}

class CheckBox extends FormControl {
}

class DateTimeBox extends FormControl {
}

class Procedure extends DersaEntity {
}

class Attribute extends DersaEntity {
    static get attributes() {
        return [
            {Name: 'PhisicalName', Type: ShortText},
            {Name: 'Description', Type: LongText}
        ];
    }
}

class Relation extends DersaRelation {

}

class Inherit extends DersaRelation {

}

const Stereotypes = {
    Package: Package,
    Configuration: Configuration,
    Type: Type,
    Const: Const,
    Script: Script,
    Procedure: Procedure,
    Entity: Entity,
    Attribute: Attribute,
    Report: Report,
    JSForm: JSForm,
    FilterForm: FilterForm,
    ComboBox: ComboBox,
    ListBox: ListBox,
    TextBox: TextBox,
    CheckBox: CheckBox,
    DateTimeBox: DateTimeBox,
    Inherit: Inherit,
    Relation: Relation
}

class ShortText {
    static getAttrObject (attr) {
        return {Name: attr.Name, Value: attr.Value? attr.Value: '', ControlType: 'text'};
    }

    static get name(){return 'ShortText';}
}

class LongText {
    static getAttrObject(attr) {
        return {Name: attr.Name, Value: attr.Value? attr.Value: '', ControlType: 'button', ChildFormAttrs: {
                Height: 900,
                Width: 600,
                DisplayValue: "...",
                formAttrs: [{Name: attr.Name, Value: attr.Value? attr.Value: '', ControlType: "textarea", Height: 350, Width: 300}],
                cbOK: result => {
                                node.setProperties(result);
                            }
            }
        };
    }
    static get name(){return 'LongText';}
}






// Аналог GetStringValueBySource: получаем значение по строке-имени (свойство или метод без аргументов)
function getStringValueBySource(obj, sourceName) {
  
if(sourceName.indexOf('()') > 0) {
  const method = obj[sourceName.replace('()', '')];
  if (typeof method === 'function') {
    try {
      const res = method.call(obj);
      if (res == null) return '';
      return String(res);
    } catch (e) {
      return '';
    }
  }
}
  if (obj.hasOwnProperty(sourceName) || sourceName in obj) {
    const val = obj[sourceName];
    if (typeof val === 'string') return val;
    if (val == null) return '';
    return String(val);
  }

  

  return '';
}

// Экранирование спецсимволов для replace (чтобы не ломать шаблон)
function escapeRegex(str) {
  return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}


/**
 * Аналог SmartReplace из C#
 * @param {string} templ - шаблон
 * @param {string|RegExp} regexpText - регулярное выражение для поиска плейсхолдеров
 * @param {any[]} objects - массив объектов для итерации (может быть null/undefined)
 * @param {string|null} lineTerminator - разделитель между итерациями (например, "\n")
 * @param {{from:string,to:string}[]} replacements - замены строк (аналог Tuple<string,string>[])
 */
function smartReplace(templ, regexpText, objects, lineTerminator, replacements) {
  const re = typeof regexpText === 'string'
    ? new RegExp(regexpText, 'g')
    : regexpText;

  if (!Array.isArray(objects)) {
    objects = [];
  }

  const hasReplacements = Array.isArray(replacements) && replacements.length > 0;
  let result = '';
  let firstEntry = true;

  for (const o of objects) {
    let current = templ;

    // Если есть совпадения по регексу — делаем подстановку
    if (re.source) {
      current = current.replace(re, (match, group1) => {
        const value = getStringValueBySource(o, group1);

        if (hasReplacements) {
          let sv = value;
          for (const r of replacements) {
            // Глобальная замена подстроки
            const escapedFrom = escapeRegex(r.from);
            const regex = new RegExp(escapedFrom, 'g');
            sv = sv.replace(regex, r.to);
          }
          return sv;
        }
        return value;
      });
    }

    if (lineTerminator != null && !firstEntry) {
      result += lineTerminator;
    }
    result += current;
    firstEntry = false;
  }

  return result;
}


/**
 * Аналог GenerateText из C#
 * @param {DersaNode} ent - текущий узел (ICompiledEntity)
 * @param {string|null} templateName - имя шаблона
 * @param {string} configName - имя конфигурации
 */
function generateText(ent, templateName, configName) {
  const typeName = ent.constructor.name; // аналог ent.GetType().Name

  let par = ent.parent; // Parent
  let P = par instanceof Package ? par : null;

  while (P === null && par != null) {
    par = par.parent;
    if (par === null) {
      return `parent of this ${typeName} must be Package`;
    }
    P = par instanceof Package ? par : null;
  }

  // Получаем конфигурацию (предполагаем, что у Package есть такой метод)
  const C = P.getConfiguration(configName);
  if (!C) {
    return 'config not found';
  }

  if (!templateName || templateName.trim() === '') {
    templateName = 'Create' + typeName;
  }

  let dst = C.getTextSetting(templateName);
  if (dst == null) dst = '';

  // Подстановка [this.X]
  const reTextQualReplaceThis = /\[this\.(.+?)\]/g;
  dst = smartReplace(dst, reTextQualReplaceThis, [ent], null, [{ from: "'", to: "''" }]);

  // Подстановка [Parent.X]
  const reTextQualReplaceParent = /\[Parent\.(.+?)\]/g;
  dst = smartReplace(dst, reTextQualReplaceParent, [P], null, [{ from: "'", to: "''" }]);

  // Обработка блоков {{method}}...{{end}}
  const regexpText = /{{(.+?)}}([\s\S]*?){{(.*?)}}/g;

  dst = dst.replace(regexpText, (match, methodName, innerContent, endTag) => {
    // Ищем в methodName конструкцию Children(TypeName)
    const childrenMatch = /Children\((.*?)\)/.exec(methodName);
    let objects = [];

    if (childrenMatch) {
      const childTypeName = childrenMatch[1].trim();
      // Фильтруем детей по имени конструктора (аналог IsInstanceOfType)
      objects = ent.children.filter(child => {
        if (!childTypeName) return true;
        return child.constructor.name === childTypeName;
      });
    } else {
      // Иначе пытаемся вызвать метод у ent
      const method = ent[methodName];
      if (typeof method !== 'function') {
        throw new Error(`метод ${methodName} не найден`);
      }
      const result = method.call(ent);
      objects = Array.isArray(result) ? result : (result || []);
    }

    const lineTerminator = (endTag || '') + '\n';
    const replacements = [{ from: "'", to: "''" }];

    return smartReplace(innerContent, /\[(.+?)\]/g, objects, lineTerminator, replacements);
  });

  return dst;
}
