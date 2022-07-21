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
    alert(e.target.id);
});