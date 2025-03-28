function searchDetailStaff() {
    const frameSearchDetail = document.querySelector(".frame-search-detail");
    const deptId = frameSearchDetail.querySelector("form input");
    const input = frameSearchDetail.querySelector(".input-group input");
    const result = frameSearchDetail.querySelector("tbody");

    // Thay đổi mỗi khi người dùng nhập
    input.addEventListener("input", function () {
        // Lưu dữ liệu ban đầu
        let listStaffs = document.querySelectorAll(".listStaffs");
        listStaffs.forEach((checkbox) => {
            if (checkbox.checked) saveChecked.add(checkbox.value);
            else saveChecked.delete(checkbox.value);
        });

        // Tiến hành gửi
        const url = `/Department/Detail?id=${deptId.value}&find=${this.value}`;
        const xhr = new XMLHttpRequest();
        xhr.open("get", url, true);
        xhr.setRequestHeader("REQUESTED", "AJAX");
        xhr.onload = function () {
            if (xhr.status == 200) {
                result.innerHTML = xhr.responseText;

                // Khôi phục
                listStaffs = document.querySelectorAll(".listStaffs");
                listStaffs.forEach((checkbox) => {
                    if (saveChecked.has(checkbox.value)) checkbox.checked = true;
                    else checkbox.checked = false;
                });
            }
        }
        xhr.send();
    });
}