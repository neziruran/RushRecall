import flet as ft
import os
import json

UPLOAD_FOLDER = "uploads"
ALLOWED_EXTENSIONS = {"json"}


def allowed_file(filename):
    return "." in filename and filename.rsplit(".", 1)[1].lower() in ALLOWED_EXTENSIONS


def main(page: ft.Page):
    page.title = "Game Data Viewer"
    page.scroll = "auto"

    # Create upload directory if needed
    if not os.path.exists(UPLOAD_FOLDER):
        try:
            os.makedirs(UPLOAD_FOLDER)
        except Exception as e:
            page.snack_bar = ft.SnackBar(ft.Text(f"Error creating upload folder: {str(e)}"))
            page.snack_bar.open = True
            page.update()

    def show_index(message=None):
        if message:
            show_message(message)

        files = []
        if os.path.exists(UPLOAD_FOLDER):
            files = [f for f in os.listdir(UPLOAD_FOLDER) if allowed_file(f)]

        file_links = []
        for file in files:
            file_links.append(
                ft.ListTile(
                    title=ft.Text(file),
                    on_click=lambda e, f=file: show_view(f),
                )
            )

        upload_button = ft.ElevatedButton(
            "Upload JSON File",
            icon=ft.icons.UPLOAD_FILE,
            on_click=lambda _: file_picker.pick_files(
                allowed_extensions=["json"],
                allow_multiple=False
            )
        )

        content = ft.Column(
            controls=[
                ft.Row([
                    ft.Text("Game Data Files", size=25),
                    upload_button
                ]),
                ft.Divider(),
                ft.Text("Available Files", size=20),
                ft.ListView(file_links, height=300) if files else ft.Text("No files available"),
            ]
        )
        page.clean()
        page.add(content)

    def show_view(filename):
        try:
            with open(os.path.join(UPLOAD_FOLDER, filename), "r") as f:
                data = json.load(f)
            json_data = json.dumps(data, indent=4)
        except:
            json_data = "Invalid JSON format"

        view_content = ft.Column(
            controls=[
                ft.Row([
                    ft.ElevatedButton("Back", on_click=lambda e: show_index()),
                    ft.Text(f"Viewing: {filename}", size=20),
                ]),
                ft.Divider(),
                ft.Container(
                    ft.Text(json_data, selectable=True),
                    bgcolor=ft.colors.GREY_100,
                    padding=15,
                    border=ft.border.all(1, ft.colors.GREY_300),
                    border_radius=5,
                )
            ]
        )
        page.clean()
        page.add(view_content)

    def show_message(message):
        page.snack_bar = ft.SnackBar(ft.Text(message))
        page.snack_bar.open = True

    def on_file_upload(e: ft.FilePickerResultEvent):
        if e.files:
            file = e.files[0]
            if allowed_file(file.name):
                try:
                    dest_path = os.path.join(UPLOAD_FOLDER, file.name)
                    # In real app, save the file content here
                    # For Unity integration, we'll handle actual file transfer later
                    show_message(f"File {file.name} ready for upload!")
                    show_index()
                except Exception as ex:
                    show_message(f"Upload error: {str(ex)}")
            else:
                show_message("Only JSON files are allowed!")

    # Setup file picker
    file_picker = ft.FilePicker(on_result=on_file_upload)
    page.overlay.append(file_picker)

    # Start with index
    show_index()


if __name__ == "__main__":
    ft.app(target=main, view=ft.WEB_BROWSER)