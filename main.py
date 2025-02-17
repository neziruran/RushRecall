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

        content = ft.Column(
            controls=[
                ft.Row([ft.Text("Game Data Files", size=25)]),
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
        except Exception as e:
            json_data = f"Invalid JSON format: {e}"

        view_content = ft.Column(
            controls=[
                ft.Row(
                    controls=[
                        ft.ElevatedButton("Back", on_click=lambda e: show_index()),
                        ft.Text(f"Viewing: {filename}", size=20),
                    ],
                    alignment=ft.MainAxisAlignment.START,
                ),
                ft.Divider(),
                ft.Container(
                    content=ft.TextField(
                        value=json_data,
                        read_only=True,
                        multiline=True,
                        expand=True,
                        filled=True,
                        fill_color=ft.colors.BLACK,
                        text_style=ft.TextStyle(font_family="monospace", color=ft.colors.WHITE),
                        border_color=ft.colors.TRANSPARENT,
                        content_padding=10,
                    ),
                    expand=True,
                    padding=15,
                    border=ft.border.all(1, ft.colors.GREY_800),
                    border_radius=5,
                )
            ],
            expand=True,
        )
        page.clean()
        page.add(view_content)

    def show_message(message):
        page.snack_bar = ft.SnackBar(ft.Text(message))
        page.snack_bar.open = True

    # Start with index
    show_index()


if __name__ == "__main__":
    ft.app(target=main, view=ft.WEB_BROWSER)
