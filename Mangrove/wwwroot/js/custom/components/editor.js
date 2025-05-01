function editWithCkEditor(idEditor) {
	const editor = document.querySelector(idEditor);
	if (editor) {
		ClassicEditor
			.create(editor, {
				ckfinder: {
					uploadUrl: '/SettingWebsite/Upload'
				},
				image: {
					toolbar: [
						'imageTextAlternative',
						'imageStyleFull',
						'imageStyleSide',
						'toggleImageCaption',
					],
				},
				toolbar: {
					items: [
						'undo', 'redo', '|',
						'heading', '|',
						'bold', 'italic', '|',
						'blockQuote', 'link', 'imageUpload', 'mediaEmbed', 'insertTable', '|',
					]
				},
				mediaEmbed: {
					previewsInData: true
				}
			})
	}
}

editWithCkEditor("#InforVi");
editWithCkEditor("#InforEn");

// Thêm class show image vào các img trong view show edior
function addClassShowImageToImageShowEditor() {
	const imgs = document.querySelectorAll(".frame_show_editor img");
	imgs.forEach((img) => {
		img.classList.add("click_show_image");
	});
}
addClassShowImageToImageShowEditor();