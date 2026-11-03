(function () {
    "use strict";

    const modalElement = document.getElementById("signalRModal");
    if (!modalElement || typeof window.signalR === "undefined") {
        return;
    }

    const statusElement = document.getElementById("hubStatus");
    const form = document.getElementById("signalRFeeForm");
    const formError = document.getElementById("signalRFormError");
    const modalTitle = document.getElementById("signalRModalLabel");
    const saveButton = document.getElementById("btnSignalRSave");
    const packageSelect = document.getElementById("srFeePackage");
    const tableBody = document.querySelector("#membershipFeeTable tbody");

    const fields = {
        id: document.getElementById("srMembershipFeeQuangTtid"),
        memberName: document.getElementById("srMemberName"),
        amount: document.getElementById("srAmount"),
        paymentDate: document.getElementById("srPaymentDate"),
        paymentMethod: document.getElementById("srPaymentMethod"),
        transactionCode: document.getElementById("srTransactionCode"),
        startDate: document.getElementById("srStartDate"),
        endDate: document.getElementById("srEndDate"),
        notes: document.getElementById("srNotes"),
        status: document.getElementById("srStatus"),
        publishDate: document.getElementById("srPublishDate"),
        isActive: document.getElementById("srIsActive")
    };

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/membershipFeeHub")
        .withAutomaticReconnect()
        .build();

    let currentId = 0;
    let packagesLoaded = false;

    function pad(value) {
        return String(value).padStart(2, "0");
    }

    function today() {
        const now = new Date();
        return now.getFullYear() + "-" + pad(now.getMonth() + 1) + "-" + pad(now.getDate());
    }

    function setStatus(message, type) {
        statusElement.className = "alert py-2 small mb-3 alert-" + (type || "secondary");
        statusElement.textContent = message;
    }

    function showError(message) {
        formError.textContent = message;
        formError.classList.remove("d-none");
    }

    function clearError() {
        formError.textContent = "";
        formError.classList.add("d-none");
    }

    function errorText(error) {
        return (error && (error.message || error.toString())) || "Unknown error";
    }

    // ---- Vẽ bảng tại chỗ từ dữ liệu server tự động đẩy về (KHÔNG reload, KHÔNG request HTTP) ----
    function escapeHtml(value) {
        if (value == null) return "";
        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function fmtDate(value) {
        if (!value) return "-";
        var m = /^(\d{4})-(\d{2})-(\d{2})/.exec(String(value));
        if (!m) return String(value);
        return m[3] + "/" + m[2] + "/" + m[1];
    }

    function fmtAmount(value) {
        var n = Number(value);
        if (!isFinite(n)) n = 0;
        return n.toLocaleString("en-US", { maximumFractionDigits: 0 });
    }

    function renderRow(item) {
        var id = item.membershipFeeQuangTtid;
        var name = escapeHtml(item.memberName || "");
        var tx = escapeHtml(item.transactionCode || "");
        var amount = fmtAmount(item.amount) + " đ";
        var method = escapeHtml(item.paymentMethod || "");
        var payDate = fmtDate(item.paymentDate);
        var start = fmtDate(item.startDate);
        var end = fmtDate(item.endDate);
        var publish = fmtDate(item.publishDate);
        var pkg = escapeHtml(item.packageName || "-");
        var status = item.status != null ? String(item.status) : "-";
        var badge = item.isActive
            ? '<span class="badge bg-success">Active</span>'
            : '<span class="badge bg-secondary">Inactive</span>';

        return '<tr>' +
            '<td><div class="fw-semibold">' + name + '</div>' +
                '<div class="text-muted small">' + tx + '</div></td>' +
            '<td><div class="fw-semibold">' + amount + '</div>' +
                '<div class="text-muted small">' + method + ' · ' + payDate + '</div></td>' +
            '<td><div>' + start + ' → ' + end + '</div>' +
                '<div class="text-muted small">Publish: ' + publish + '</div></td>' +
            '<td>' + pkg + '</td>' +
            '<td><div class="small">Status: ' + status + '</div>' + badge + '</td>' +
            '<td class="text-end"><div class="d-flex flex-wrap gap-1 justify-content-end">' +
                '<a href="/MembershipFees/Details?id=' + id + '" class="btn btn-sm btn-outline-secondary">Details</a>' +
                '<a href="/MembershipFees/Edit?id=' + id + '" class="btn btn-sm btn-outline-primary">Edit</a>' +
                '<a href="/MembershipFees/Delete?id=' + id + '" class="btn btn-sm btn-outline-danger">Delete</a>' +
                '<button type="button" class="btn btn-sm btn-success btn-sr-edit" data-id="' + id + '">SignalR Edit</button>' +
                '<button type="button" class="btn btn-sm btn-outline-warning btn-sr-delete" data-id="' + id + '">SignalR Delete</button>' +
            '</div></td>' +
        '</tr>';
    }

    function initPagination() {
        if (typeof window.tablePagination === "undefined") return;
        window.tablePagination.init({
            table: "#membershipFeeTable",
            pager: "#feePager",
            info: "#feePageInfo",
            pageSizeSelector: "#feePageSize",
            pageSize: 10
        });
    }

    function renderTable(items) {
        if (!tableBody) return;
        var html = "";
        if (!items || items.length === 0) {
            html = '<tr class="js-empty-row"><td colspan="6" class="text-center text-muted py-4">Không có dữ liệu phù hợp.</td></tr>';
        } else {
            for (var i = 0; i < items.length; i++) {
                html += renderRow(items[i]);
            }
        }
        // Chỉ ghi lại các dòng trong tbody — giữ nguyên element tbody để delegate click vẫn hoạt động.
        tableBody.innerHTML = html;
        initPagination();
    }

    function resetForm() {
        currentId = 0;
        fields.id.value = "0";
        fields.memberName.value = "";
        fields.amount.value = "";
        fields.paymentDate.value = today();
        fields.paymentMethod.value = "";
        fields.transactionCode.value = "";
        fields.startDate.value = "";
        fields.endDate.value = "";
        fields.notes.value = "";
        fields.status.value = "";
        fields.publishDate.value = today();
        fields.isActive.checked = true;
        packageSelect.value = "";
        modalTitle.textContent = "SignalR Create";
        saveButton.textContent = "Create via SignalR";
        clearError();
    }

    function fillForm(item) {
        currentId = Number(item.membershipFeeQuangTtid) || 0;
        fields.id.value = String(currentId);
        fields.memberName.value = item.memberName || "";
        fields.amount.value = item.amount != null ? item.amount : "";
        fields.paymentDate.value = item.paymentDate || "";
        fields.paymentMethod.value = item.paymentMethod || "";
        fields.transactionCode.value = item.transactionCode || "";
        fields.startDate.value = item.startDate || "";
        fields.endDate.value = item.endDate || "";
        fields.notes.value = item.notes || "";
        fields.status.value = item.status != null ? item.status : "";
        fields.publishDate.value = item.publishDate || "";
        fields.isActive.checked = !!item.isActive;

        if (packagesLoaded) {
            packageSelect.value = item.feePackageQuangTtid != null
                ? String(item.feePackageQuangTtid)
                : "";
        }

        modalTitle.textContent = "SignalR Edit #" + currentId;
        saveButton.textContent = "Update via SignalR";
        clearError();
    }

    function readForm() {
        return {
            membershipFeeQuangTtid: currentId,
            memberName: fields.memberName.value.trim(),
            amount: Number(fields.amount.value),
            paymentDate: fields.paymentDate.value || null,
            paymentMethod: fields.paymentMethod.value || null,
            transactionCode: fields.transactionCode.value || null,
            startDate: fields.startDate.value || null,
            endDate: fields.endDate.value || null,
            notes: fields.notes.value || null,
            status: fields.status.value === "" ? null : Number(fields.status.value),
            publishDate: fields.publishDate.value || null,
            feePackageQuangTtid: packageSelect.value === "" ? null : Number(packageSelect.value),
            isActive: fields.isActive.checked
        };
    }

    function appendPackageOption(value, label) {
        const option = document.createElement("option");
        option.value = value;
        option.textContent = label;
        packageSelect.appendChild(option);
    }

    async function loadPackages() {
        const packages = await connection.invoke("GetPackages");
        packageSelect.innerHTML = "";
        appendPackageOption("", "-- Chọn gói --");

        (packages || []).forEach(function (item) {
            const id = item.feePackageQuangTtid;
            const name = item.packageName || ("Package #" + id);
            appendPackageOption(String(id), name);
        });

        packagesLoaded = true;
    }

    function showModal() {
        bootstrap.Modal.getOrCreateInstance(modalElement).show();
    }

    async function openEdit(id) {
        clearError();

        try {
            const item = await connection.invoke("GetById", id);
            if (!item) {
                setStatus("Không tìm thấy khoản phí #" + id + ".", "warning");
                return;
            }

            if (!packagesLoaded) {
                await loadPackages();
            }

            fillForm(item);
            showModal();
        } catch (error) {
            setStatus("Không tải được khoản phí #" + id + ": " + errorText(error), "danger");
        }
    }

    async function deleteFee(id) {
        if (!window.confirm("Xoá khoản phí #" + id + " bằng SignalR?")) {
            return;
        }

        try {
            setStatus("Đang xoá khoản phí #" + id + " qua SignalR...", "secondary");
            await connection.invoke("Delete", id);
            // Server sẽ push danh sách mới về — bảng tự cập nhật, KHÔNG reload.
        } catch (error) {
            setStatus("Xoá thất bại: " + errorText(error), "danger");
        }
    }

    async function refreshFromServer() {
        try {
            const items = await connection.invoke("GetAll");
            renderTable(items || []);
        } catch (error) {
            setStatus("Không tải được danh sách: " + errorText(error), "danger");
        }
    }

    // Server tự động đẩy toàn bộ danh sách mới nhất về MỌI tab.
    // Vẽ lại bảng tại chỗ — tab Network KHÔNG xuất hiện request nào mới.
    connection.on("MembershipFeesChanged", function (payload) {
        const items = (payload && payload.items) ? payload.items : [];
        renderTable(items);

        const action = (payload && payload.action) ? payload.action : "changed";
        const memberName = (payload && payload.memberName) ? payload.memberName : "";
        const changedAt = (payload && payload.changedAt) ? payload.changedAt : "";
        setStatus("SignalR server đang cập nhật: " + action + " \"" + memberName + "\" lúc " + changedAt + ".", "success");
    });

    connection.onreconnecting(function () {
        setStatus("SignalR đang kết nối lại...", "warning");
    });

    connection.onreconnected(function () {
        setStatus("SignalR đã kết nối lại.", "success");
        loadPackages().catch(function (error) {
            setStatus("Không tải được gói phí: " + errorText(error), "danger");
        });
        refreshFromServer();
    });

    connection.onclose(function () {
        setStatus("SignalR đã ngắt kết nối. Tải lại trang để thử lại.", "danger");
    });

    document.getElementById("btnSignalRCreate").addEventListener("click", function () {
        resetForm();
    });

    form.addEventListener("submit", async function (event) {
        event.preventDefault();
        clearError();

        const dto = readForm();
        if (!dto.memberName) {
            showError("Vui lòng nhập tên hội viên.");
            fields.memberName.focus();
            return;
        }

        if (!Number.isFinite(dto.amount) || dto.amount <= 0) {
            showError("Số tiền phải lớn hơn 0.");
            fields.amount.focus();
            return;
        }

        const isUpdate = currentId > 0;
        saveButton.disabled = true;
        saveButton.textContent = "Đang lưu...";

        try {
            if (isUpdate) {
                await connection.invoke("Update", dto);
                setStatus("Đã cập nhật khoản phí #" + currentId + " qua SignalR.", "success");
            } else {
                await connection.invoke("Create", dto);
                setStatus("Đã tạo khoản phí mới qua SignalR.", "success");
            }

            const modal = bootstrap.Modal.getInstance(modalElement);
            if (modal) {
                modal.hide();
            }

            // Server push sẽ cập nhật bảng cho mọi tab — KHÔNG reload, KHÔNG request HTTP.
        } catch (error) {
            showError("Lưu thất bại: " + errorText(error));
        } finally {
            saveButton.disabled = false;
            saveButton.textContent = currentId > 0 ? "Update via SignalR" : "Create via SignalR";
        }
    });

    if (tableBody) {
        tableBody.addEventListener("click", function (event) {
            const editButton = event.target.closest(".btn-sr-edit");
            if (editButton) {
                openEdit(Number(editButton.dataset.id));
                return;
            }

            const deleteButton = event.target.closest(".btn-sr-delete");
            if (deleteButton) {
                deleteFee(Number(deleteButton.dataset.id));
            }
        });
    }

    resetForm();

    connection.start()
        .then(async function () {
            setStatus("SignalR đã kết nối. Các nút CRUD SignalR đã sẵn sàng.", "success");
            try {
                await loadPackages();
            } catch (error) {
                setStatus("Không tải được danh sách gói phí: " + errorText(error), "danger");
            }
        })
        .catch(function (error) {
            setStatus("Kết nối SignalR thất bại: " + errorText(error), "danger");
        });
})();
