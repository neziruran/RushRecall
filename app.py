from flask import Flask, render_template, flash, redirect, url_for
import os
import json

app = Flask(__name__)
app.secret_key = 'test'  # Still needed for flash messages

# Configuration
UPLOAD_FOLDER = 'uploads'
ALLOWED_EXTENSIONS = {'json'}
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER

# Create upload directory if it doesn't exist
if not os.path.exists(UPLOAD_FOLDER):
    try:
        os.makedirs(UPLOAD_FOLDER)
    except OSError as e:
        flash(f'Error creating upload folder: {str(e)}')


def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS


@app.route('/')
def index():
    if not os.path.exists(app.config['UPLOAD_FOLDER']):
        flash('Upload folder is not available')
        return render_template('index.html', files=[])

    files = os.listdir(app.config['UPLOAD_FOLDER'])
    files = [f for f in files if allowed_file(f)]
    return render_template('index.html', files=files)


@app.route('/view/<filename>')
def view_file(filename):
    file_path = os.path.join(app.config['UPLOAD_FOLDER'], filename)
    if not os.path.exists(file_path):
        flash('File not found')
        return redirect(url_for('index'))

    try:
        with open(file_path, 'r') as f:
            data = json.load(f)
        json_data = json.dumps(data, indent=4)
    except json.JSONDecodeError:
        json_data = "Invalid JSON format"

    return render_template('view.html', filename=filename, json_data=json_data)


if __name__ == '__main__':
    app.run(debug=False, host='0.0.0.0')