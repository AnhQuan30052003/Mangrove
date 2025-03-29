// Save danh sách của đối tượng
export function saveIndex(key = null) {
	const url = location.href;
	localStorage.setItem(key != null ? key : "urlIndex", url);
}

// Lấy ra url index
export function getUrlIndex(key = null) {
	const url = localStorage.getItem(key != null ? key : "urlIndex");
	if (url != null) {
		location.href = url;
		localStorage.removeItem("urlIndex");
	}
}
