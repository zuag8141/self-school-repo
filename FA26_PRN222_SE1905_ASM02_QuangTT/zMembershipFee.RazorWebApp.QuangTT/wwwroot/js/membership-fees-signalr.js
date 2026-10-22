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
    let reloadScheduled = false;

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

    function scheduleReload() {
        if (reloadScheduled) {
            return;
        }

        reloadScheduled = true;
        window.setTimeout(function () {
            window.location.reload();
        }, 700);
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
            setStatus("Đã xoá khoản phí #" + id + " qua SignalR.", "success");
            scheduleReload();
        } catch (error) {
            setStatus("Xoá thất bại: " + errorText(error), "danger");
        }
    }

    connection.on("MembershipFeeChanged", function (payload) {
        const memberName = payload && payload.memberName ? payload.memberName : "";
        const action = payload && payload.action ? payload.action : "changed";
        const changedAt = payload && payload.changedAt ? payload.changedAt : "";
        setStatus("SignalR: " + action + " \"" + memberName + "\" lúc " + changedAt + ". Đang tải lại...", "success");
        scheduleReload();
    });

    connection.onreconnecting(function () {
        setStatus("SignalR đang kết nối lại...", "warning");
    });

    connection.onreconnected(function () {
        setStatus("SignalR đã kết nối lại.", "success");
        loadPackages().catch(function (error) {
            setStatus("Không tải được gói phí: " + errorText(error), "danger");
        });
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

            scheduleReload();
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
