(() => {
    const root = document.querySelector('[data-live-fee-id]');
    if (!root || !window.signalR) return;
    const id = Number(root.dataset.liveFeeId);
    const connection = new signalR.HubConnectionBuilder().withUrl('/membershipFeeHub').withAutomaticReconnect().build();
    function render(item) {
        if (!item) {
            root.querySelectorAll('input,select,textarea,button').forEach(x => x.disabled = true);
            if (!root.querySelector('[role="alert"]')) {
                const message = document.createElement('p'); message.className = 'alert alert-warning'; message.role = 'alert';
                message.textContent = 'Khoản phí này đã được xoá.'; root.prepend(message);
            }
            return;
        }
        Object.entries(item).forEach(([key,value]) => {
            const name = key[0].toUpperCase() + key.slice(1);
            const input = root.querySelector('[name="' + name + '"]');
            if (input) { if(input.type === 'checkbox') input.checked = !!value; else input.value = value ?? ''; }
        });
        const date = value => value ? value.slice(0,10).split('-').reverse().join('/') : '-';
        const cells = root.querySelectorAll('dd');
        const values = [item.memberName, Number(item.amount).toLocaleString() + ' đ', date(item.paymentDate),
            item.paymentMethod, item.transactionCode, item.packageName,
            date(item.startDate) + ' → ' + date(item.endDate),
            (item.status ?? '-') + ' / ' + (item.isActive ? 'Active' : 'Inactive'),
            date(item.publishDate), item.updateAt, item.notes];
        cells.forEach((cell,index) => { if(index < values.length) cell.textContent = values[index] ?? '-'; });
    }
    connection.on('MembershipFeesChanged', payload => render(payload.items.find(x => x.membershipFeeQuangTtid === id)));
    connection.onreconnected(async () => render(await connection.invoke('GetById',id)));
    connection.start().then(async () => render(await connection.invoke('GetById',id))).catch(console.error);
})();
