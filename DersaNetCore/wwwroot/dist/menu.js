document.oncontextmenu = function () { return false; };
$(document).ready(function () {

    // Вешаем слушатель события нажатие кнопок мыши для всего документа:
    $(document).mousedown(function (event) {

        // Убираем css класс selected-html-element у абсолютно всех элементов на странице с помощью селектора "*":
        $('*').removeClass('selected-html-element');

        // Проверяем нажата ли именно правая кнопка мыши:
        if (event.which === 3) {

            MenuItemText = 'Refresh';
            if (event.target.id[0] == '-')
                MenuItemText = 'Find entity B';
            if (event.target.id[0] == "D")
                MenuItemText = 'Edit';
            $('.context-menu').remove();

            // Получаем элемент на котором был совершен клик:
            var target = $(event.target);
            if (!CanShow(event))
                return;
            var displaytext = document.getElementById("displaytext");
            displaytext.value = event.target.id;

            // Добавляем класс selected-html-element что бы наглядно показать на чем именно мы кликнули (исключительно для тестирования):
            target.addClass('selected-html-element');

            // Создаем меню:
            $('<div/>', {
                id: 'contmenu',
                class: 'context-menu' // Присваиваем блоку наш css класс контекстного меню:
            })
            .css({
                left: event.pageX + 'px', // Задаем позицию меню на X
                top: event.pageY + 'px' // Задаем позицию меню по Y
            })
            .appendTo('body') // Присоединяем наше меню к body документа:

            .append( // Добавляем пункты меню:
                     $('<ul/>').append('<li><a href="#1" onclick="OnMenuItemClick()">' + MenuItemText + '</li>')
                    )
            .show('fast'); // Показываем меню с небольшим стандартным эффектом jQuery. Как раз очень хорошо подходит для меню
        }
    });
});
