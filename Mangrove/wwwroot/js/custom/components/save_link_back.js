// Save danh sách của đối tượng
export function saveIndex() {
	const url = location.href;
	localStorage.setItem("urlIndex", url);
}

// Lấy ra url index
export function getUrlIndex() {
	const url = localStorage.getItem("urlIndex");
	location.href = url;
}