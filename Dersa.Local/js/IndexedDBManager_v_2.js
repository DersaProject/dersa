class IndexedDBManager {
  /**
   * Конструктор класса IndexedDBManager
   * @param {string} dbName - имя базы данных
   * @param {number} version - версия базы данных
   * @param {string[]} storeNames - массив имён хранилищ, которые нужно создать
   * @param {boolean} forDebug - флаг включения отладочных сообщений (по умолчанию false)
   */
  constructor(dbName = 'DefaultDB', version = 1, storeNames = ['default_store'], forDebug = false) {
    this.dbName = dbName;
    this.version = version;
    this.storeNames = [...new Set(storeNames)]; // Убираем дубликаты
    this.db = null;
    this.initPromise = null;
    this.forDebug = forDebug;
  }

  /**
   * Вспомогательный метод для вывода отладочных сообщений
   * @param {...any} args - аргументы для console.log
   */
  _debugLog(...args) {
    if (this.forDebug) {
      console.log(...args);
    }
  }

  /**
   * Инициализация соединения с базой данных
   * @returns {Promise<IDBDatabase>}
   */
  async init() {
    if (this.initPromise) {
      return this.initPromise;
    }

    this.initPromise = new Promise((resolve, reject) => {
      this._debugLog(`Попытка открыть БД: "${this.dbName}", версия: ${this.version}`);
      this._debugLog('Требуемые хранилища:', this.storeNames);

      const request = indexedDB.open(this.dbName, this.version);

      request.onerror = (event) => {
        console.error('Ошибка открытия БД:', event.target.error);
        reject(new Error(`Ошибка открытия БД: ${event.target.error}`));
        this.initPromise = null;
      };

      request.onsuccess = (event) => {
        this.db = event.target.result;
        this._debugLog('БД успешно открыта:', this.db.name, 'версия:', this.db.version);
        this._debugLog('Доступные хранилища:', Array.from(this.db.objectStoreNames));
        resolve(this.db);
      };

      request.onupgradeneeded = (event) => {
        const db = event.target.result;
        this._debugLog('Запуск onupgradeneeded, версия:', this.version);

        // Создаём все требуемые хранилища
        for (const storeName of this.storeNames) {
          if (!db.objectStoreNames.contains(storeName)) {
            db.createObjectStore(storeName, { keyPath: 'id' });
            this._debugLog(`Создано хранилище: ${storeName}`);
          } else {
            this._debugLog(`Хранилище уже существует: ${storeName}`);
          }
        }
      };

      // Таймаут на случай зависания запроса (10 секунд)
      setTimeout(() => {
        if (!this.db) {
          const errorMsg = 'Таймаут инициализации БД (10 с)';
          console.error(errorMsg);
          reject(new Error(errorMsg));
          this.initPromise = null;
        }
      }, 10000);
    });

    return this.initPromise;
  }

  /**
   * Сохранение объекта данных в указанное хранилище
   * @param {string} storeName - имя хранилища
   * @param {string|number} key - ключ записи
   * @param {Object} data - объект данных для сохранения
   * @returns {Promise<void>}
   */
  async saveData(storeName, key, data) {
    this._debugLog('saveData: начало, хранилище:', storeName, 'ключ:', key);

    if (!this.db) {
      this._debugLog('saveData: БД не инициализирована, вызываем init()');
      await this.init();
      this._debugLog('saveData: init() завершён');
    }

    // Проверяем, что хранилище существует
    if (!this.storeNames.includes(storeName)) {
      throw new Error(`Хранилище "${storeName}" не было указано при создании экземпляра класса`);
    }

    // Гарантируем, что data — это объект
    if (typeof data !== 'object' || data === null) {
      throw new Error('Данные должны быть объектом');
    }

    return new Promise((resolve, reject) => {
      try {
        const transaction = this.db.transaction([storeName], 'readwrite');
        const store = transaction.objectStore(storeName);
        const item = { id: key, data: data };
        const request = store.put(item);

        request.onsuccess = () => {
          this._debugLog('saveData: данные сохранены в хранилище:', storeName);
          resolve();
        };

        request.onerror = () => {
          console.error('saveData: ошибка сохранения:', request.error);
          reject(new Error(`Ошибка сохранения: ${request.error}`));
        };
      } catch (error) {
        console.error('saveData: исключение:', error);
        reject(error);
      }
    });
  }

  /**
   * Получение объекта данных из указанного хранилища по ключу
   * @param {string} storeName - имя хранилища (object store)
   * @param {string|number} key - ключ записи
   * @returns {Promise<Object|null>} - сохранённый объект или null, если не найден
   */
  async getData(storeName, key) {
    this._debugLog('getData: начало, хранилище:', storeName, 'ключ:', key);

    if (!this.db) {
      this._debugLog('getData: БД не инициализирована, вызываем init()');
      await this.init();
      this._debugLog('getData: init() завершён');
    }

    // Проверяем, что хранилище существует
    if (!this.storeNames.includes(storeName)) {
      throw new Error(`Хранилище "${storeName}" не было указано при создании экземпляра класса`);
    }

    return new Promise((resolve, reject) => {
      try {
        const transaction = this.db.transaction([storeName], 'readonly');
        const store = transaction.objectStore(storeName);
        const request = store.get(key);

        request.onsuccess = () => {
          const result = request.result;
          if (result) {
            const data = result.data;
            this._debugLog('getData: данные получены:', data);
            resolve(data);
          } else {
            this._debugLog('getData: запись не найдена');
            resolve(null);
          }
        };

        request.onerror = () => {
          console.error('getData: ошибка получения:', request.error);
          reject(new Error(`Ошибка при получении данных: ${request.error}`));
        };
      } catch (error) {
        console.error('getData: исключение:', error);
        reject(error);
      }
    });
  }

  /**
   * Удаление данных из указанного хранилища по ключу
   * @param {string} storeName - имя хранилища
   * @param {string|number} key - ключ записи для удаления
   * @returns {Promise<void>}
   */
  async deleteData(storeName, key) {
    this._debugLog('deleteData: начало, хранилище:', storeName, 'ключ:', key);

    if (!this.db) {
      this._debugLog('deleteData: БД не инициализирована, вызываем init()');
      await this.init();
      this._debugLog('deleteData: init() завершён');
    }

    // Проверяем, что хранилище существует
    if (!this.storeNames.includes(storeName)) {
      throw new Error(`Хранилище "${storeName}" не было указано при создании экземпляра класса`);
    }

    return new Promise((resolve, reject) => {
      try {
        const transaction = this.db.transaction([storeName], 'readwrite');
        const store = transaction.objectStore(storeName);
        const request = store.delete(key);

        request.onsuccess = () => {
          this._debugLog('deleteData: данные удалены из хранилища:', storeName, 'ключ:', key);
          resolve();
        };

        request.onerror = () => {
          console.error('deleteData: ошибка удаления:', request.error);
          reject(new Error(`Ошибка удаления: ${request.error}`));
        };
      } catch (error) {
        console.error('deleteData: исключение:', error);
        reject(error);
      }
    });
  }

  /**
   * Поиск записей в хранилище по ключу и значению (работает только с объектами в data)
   * @param {string} storeName - имя хранилища
   * @param {string} key - ключ для поиска (ищется внутри объекта data)
   * @param {*} value - значение для поиска
   * @returns {Promise<Array<Object>>} - массив найденных объектов (только поле data)
   */
  async findByKeyValue(storeName, key, value) {
    this._debugLog('findByKeyValue: начало, хранилище:', storeName, 'ключ:', key, 'значение:', value);

    if (!this.db) {
      this._debugLog('findByKeyValue: БД не инициализирована, вызываем init()');
      await this.init();
      this._debugLog('findByKeyValue: init() завершён');
    }

    // Проверяем, что хранилище существует
    if (!this.storeNames.includes(storeName)) {
      throw new Error(`Хранилище "${storeName}" не было указано при создании экземпляра класса`);
    }

    return new Promise((resolve, reject) => {
      try {
        const transaction = this.db.transaction([storeName], 'readonly');
        const store = transaction.objectStore(storeName);
        const request = store.openCursor();
        const results = [];

        request.onsuccess = (event) => {
          const cursor = event.target.result;
          if (cursor) {
            const item = cursor.value;

            // Гарантируем, что data — это объект
            if (typeof item.data === 'object' && item.data !== null) {
              // Проверяем наличие ключа и совпадение значения
              if (item.data.hasOwnProperty(key) && item.data[key] === value) {
                results.push(item.data); // Сохраняем только поле data
              }
            }

            // Продолжаем перебор
            cursor.continue();
          } else {
            // Курсор достиг конца — возвращаем результаты
            this._debugLog('findByKeyValue: найдено записей:', results.length);
            resolve(results);
          }
        };

        request.onerror = () => {
          console.error('findByKeyValue: ошибка поиска:', request.error);
          reject(new Error(`Ошибка поиска: ${request.error}`));
        };
      } catch (error) {
        console.error('findByKeyValue: исключение:', error);
        reject(error);
      }
    });
  }
  
  /**
   * Получить все записи из хранилища (включая id)
   * @param {string} storeName - имя хранилища (object store)
   * @returns {Promise<Array<Object>>} - массив записей вида [{ id: ..., data: ... }, ...]
   */
  async getAllData(storeName) {
    this._debugLog('getAllData: начало, хранилище:', storeName);

    if (!this.db) {
      this._debugLog('getAllData: БД не инициализирована, вызываем init()');
      await this.init();
      this._debugLog('getAllData: init() завершён');
    }

    if (!this.storeNames.includes(storeName)) {
      throw new Error(`Хранилище "${storeName}" не было указано при создании экземпляра класса`);
    }

    return new Promise((resolve, reject) => {
      try {
        const transaction = this.db.transaction([storeName], 'readonly');
        const store = transaction.objectStore(storeName);
        const request = store.getAll();

        request.onsuccess = () => {
          const records = request.result;
          this._debugLog('getAllData: получено записей:', records.length);
          resolve(records); // Возвращаем как есть: [{ id, data }, ...]
        };

        request.onerror = () => {
          console.error('getAllData: ошибка получения всех данных:', request.error);
          reject(new Error(`Ошибка получения данных: ${request.error}`));
        };
      } catch (error) {
        console.error('getAllData: исключение:', error);
        reject(error);
      }
    });
  }
  
}
