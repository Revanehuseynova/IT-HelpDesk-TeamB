// Static prototype data & rendering for prototype/index.html and prototype/tickets.html.
// The real application uses the Razor views; this file only demonstrates the UI without a server.
(function () {
    'use strict';

    var tickets = [
        { id: 1, title: 'Cannot connect to campus Wi-Fi', category: 'Network / Internet', priority: 'High', status: 'New' },
        { id: 2, title: 'Forgot student portal password', category: 'Account & Password', priority: 'Medium', status: 'In Progress' },
        { id: 3, title: 'Printer on 2nd floor is jammed', category: 'Printer', priority: 'Medium', status: 'Resolved' },
        { id: 4, title: 'Install Visual Studio in lab 3', category: 'Software', priority: 'Medium', status: 'Closed' },
        { id: 5, title: 'Monitor flickers', category: 'Hardware', priority: 'High', status: 'In Progress' }
    ];
    var statusClass = { 'New': 'text-bg-primary', 'In Progress': 'text-bg-warning', 'Resolved': 'text-bg-success', 'Closed': 'text-bg-secondary' };

    function badge(text, map) {
        var span = document.createElement('span');
        span.className = 'badge ' + (map[text] || 'text-bg-light');
        span.textContent = text;
        return span;
    }

    // Statistics (index.html)
    var stats = document.getElementById('proto-stats');
    if (stats) {
        var statuses = ['New', 'In Progress', 'Resolved', 'Closed'];
        var items = [{ label: 'Total', value: tickets.length }].concat(statuses.map(function (s) {
            return { label: s, value: tickets.filter(function (t) { return t.status === s; }).length };
        }));
        items.forEach(function (it) {
            var col = document.createElement('div');
            col.className = 'col-6 col-lg';
            col.innerHTML = '<div class="card stat-card h-100"><div class="card-body text-center"><div class="stat-number"></div><div class="text-muted"></div></div></div>';
            col.querySelector('.stat-number').textContent = it.value;
            col.querySelector('.text-muted').textContent = it.label;
            stats.appendChild(col);
        });
    }

    // Ticket list with filters (tickets.html)
    var body = document.getElementById('proto-tickets');
    if (body) {
        var statusSel = document.getElementById('proto-status');
        var catSel = document.getElementById('proto-category');
        var render = function () {
            body.innerHTML = '';
            tickets.filter(function (t) {
                return (!statusSel.value || t.status === statusSel.value) && (!catSel.value || t.category === catSel.value);
            }).forEach(function (t) {
                var tr = document.createElement('tr');
                [t.id, t.title, t.category].forEach(function (v) {
                    var td = document.createElement('td'); td.textContent = v; tr.appendChild(td);
                });
                var tdP = document.createElement('td'); tdP.textContent = t.priority; tr.appendChild(tdP);
                var tdS = document.createElement('td'); tdS.appendChild(badge(t.status, statusClass)); tr.appendChild(tdS);
                body.appendChild(tr);
            });
        };
        Array.from(new Set(tickets.map(function (t) { return t.category; }))).forEach(function (c) {
            var o = document.createElement('option'); o.value = c; o.textContent = c; catSel.appendChild(o);
        });
        statusSel.addEventListener('change', render);
        catSel.addEventListener('change', render);
        render();
    }
})();
