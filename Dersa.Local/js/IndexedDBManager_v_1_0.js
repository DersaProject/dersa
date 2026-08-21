class IndexedDBManager {
  /**
   * Конструктор класса IndexedDBManager
   * @param {string} dbName - имя базы данных
   * @param {number} version - версия базы данных (начнётся с указанного числа)
   * @param {string[]} storeNames - массив имён хранилищ, которые нужно создать
   */
  constructor(dbName = 'DefaultDB', version = 1, storeNames = ['default_store']) {
    this.dbName = dbName;
    this.version = version;
    this.storeNames = [...new Set(storeNames)]; // Убираем дубликаты
    this.db = null;
    this.initPromise = null;
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
      console.log(`Попытка открыть БД: "${this.dbName}", версия: ${this.version}`);
      console.log('Требуемые хранилища:', this.storeNames);

      const request = indexedDB.open(this.dbName, this.version);

      request.onerror = (event) => {
        console.error('Ошибка открытия БД:', event.target.error);
        reject(new Error(`Ошибка открытия БД: ${event.target.error}`));
        this.initPromise = null;
      };

      request.onsuccess = (event) => {
        this.db = event.target.result;
        console.log('БД успешно открыта:', this.db.name, 'версия:', this.db.version);
        console.log('Доступные хранилища:', Array.from(this.db.objectStoreNames));
        resolve(this.db);
      };

      request.onupgradeneeded = (event) => {
        const db = event.target.result;
        console.log('Запуск onupgradeneeded, версия:', this.version);

        // Создаём все требуемые хранилища
        for (const storeName of this.storeNames) {
          if (!db.objectStoreNames.contains(storeName)) {
            db.createObjectStore(storeName, { keyPath: 'id' });
            console.log(`Создано хранилище: ${storeName}`);
          } else {
            console.log(`Хранилище уже существует: ${storeName}`);
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
   * Сохранение данных в указанное хранилище
   * @param {string} storeName - имя хранилища
   * @param {string|number} key - ключ записи
   * @param {*} data - данные для сохранения (любой тип)
   * @returns {Promise<void>}
   */
  async saveData(storeName, key, data) {
    console.log('saveData: начало, хранилище:', storeName, 'ключ:', key);

    if (!this.db) {
      console.log('saveData: БД не инициализирована, вызываем init()');
      await this.init();
      console.log('saveData: init() завершён');
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
          console.log('saveData: данные сохранены в хранилище:', storeName);
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
    console.log('getData: начало, хранилище:', storeName, 'ключ:', key);

    if (!this.db) {
      console.log('getData: БД не инициализирована, вызываем init()');
      await this.init();
      console.log('getData: init() завершён');
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
            console.log('getData: данные получены:', data);
            resolve(data);
          } else {
            console.log('getData: запись не найдена');
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

