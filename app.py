from flask import Flask, render_template, request, flash, redirect, url_for, session
from werkzeug.utils import secure_filename
import os
import json

app = Flask(__name__)
app.secret_key = 'test'  # Replace with a strong secret key in production

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

# Hardcoded credentials for demo purposes (replace with proper authentication in production)
ADMIN_CREDENTIALS = {'username': 'admin', 'password': 'admin123'}


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


@app.route('/login', methods=['GET', 'POST'])
def login():
    if request.method == 'POST':
        username = request.form.get('username')
        password = request.form.get('password')

        if username == ADMIN_CREDENTIALS['username'] and password == ADMIN_CREDENTIALS['password']:
            session['username'] = username
            session['role'] = 'admin'
            return redirect(url_for('index'))
        else:
            flash('Invalid credentials')
    return render_template('login.html')


@app.route('/logout')
def logout():
    session.clear()
    return redirect(url_for('index'))


@app.route('/guest')
def guest():
    session['username'] = 'Guest'
    session['role'] = 'guest'
    return redirect(url_for('index'))


@app.route('/upload', methods=['POST'])
def upload_file():
    if 'role' not in session or session['role'] != 'admin':
        flash('Unauthorized access')
        return redirect(url_for('index'))

    if 'jsonfile' not in request.files:
        flash('No file part')
        return redirect(url_for('index'))

    file = request.files['jsonfile']
    if file.filename == '':
        flash('No selected file')
        return redirect(url_for('index'))

    if file and allowed_file(file.filename):
        filename = secure_filename(file.filename)
        file.save(os.path.join(app.config['UPLOAD_FOLDER'], filename))
        flash(f'File {filename} uploaded successfully')
    else:
        flash('Only JSON files are allowed')

    return redirect(url_for('index'))


@app.route('/delete_file/<filename>', methods=['POST'])
def delete_file(filename):
    if 'role' not in session or session['role'] != 'admin':
        flash('Unauthorized access')
        return redirect(url_for('index'))

    file_path = os.path.join(app.config['UPLOAD_FOLDER'], filename)
    if os.path.exists(file_path):
        os.remove(file_path)
        flash(f'File {filename} deleted successfully')
    else:
        flash('File not found')

    return redirect(url_for('index'))


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