var util = {
    scrollTop: (dom, id) => {
        if (id) {
            document.getElementById(id).scrollTop = 0;
        }
        else {
            dom.scrollTop = 0;
        }
    }
}

window.util = util;
