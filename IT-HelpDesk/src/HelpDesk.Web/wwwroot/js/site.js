// IT Help Desk - shared UI behaviour (Frontend Developer)
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        // 1. Success alerts disappear after 5 seconds.
        document.querySelectorAll('.js-auto-dismiss').forEach(function (alertEl) {
            setTimeout(function () {
                if (window.bootstrap && window.bootstrap.Alert) {
                    window.bootstrap.Alert.getOrCreateInstance(alertEl).close();
                } else {
                    alertEl.remove();
                }
            }, 5000);
        });

        // 2. Filter dropdowns submit their form on change.
        document.querySelectorAll('select[data-autosubmit]').forEach(function (select) {
            select.addEventListener('change', function () {
                if (select.form) select.form.submit();
            });
        });

        // 3. Live character counter for textareas: <textarea data-counter="#id">
        document.querySelectorAll('textarea[data-counter]').forEach(function (area) {
            var target = document.querySelector(area.getAttribute('data-counter'));
            if (!target) return;
            var update = function () { target.textContent = area.value.length; };
            area.addEventListener('input', update);
            update();
        });
    });
})();
