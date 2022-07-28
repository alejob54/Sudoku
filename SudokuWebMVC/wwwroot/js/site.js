const OnEvent = (doc) => {
    return {
        on: (type, selector, callback) => {
            doc.addEventListener(type, (event) => {
                if (!event.target.matches(selector)) return;
                callback.call(event.target, event);
            }, false);
        }
    }
};


OnEvent(document).on('click', 'td', function (e) {
    var input = prompt('Add a number');
    if (!isNaN(input)) {
        if (input >= 0 && input < 10) {
            $("#".concat(e.target.id)).html(input);
        }
    }
});