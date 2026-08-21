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
   * Поиск записей в хранилище по ключу и значению
   * @param {string} storeName - имя хранилища
   * @param {string} key - ключ для поиска
   * @param {*} value - значение для поиска
   * @returns {Promise<Array>} - массив найденных записей
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
			this._debugLog('item:' + JSON.stringify(item));
            // Проверяем, есть ли в записи нужный ключ и совпадает ли значение
            if (item.data && item.data[key] === value) {
              results.push(item);
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
   * Сохранение данных в указанное хранилище
   * @param {string} storeName - имя хранилища
   * @param {string|number} key - ключ записи
   * @param {*} data - данные для сохранения (любой тип)
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
   * Получение данных из указанного хранилища по ключу
   * @param {string} storeName - имя хранилища (object store)
   * @param {string|number} key - ключ записи
   * @returns {Promise<*|null>} - сохранённые данные или null, если не найдены
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
}
