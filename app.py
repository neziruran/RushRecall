from flask import Flask, render_template, request, redirect, url_for, flash, session
import os
import json
from werkzeug.utils import secure_filename

app = Flask(__name__)
app.secret_key = 'daimonprox'  # Replace with a strong secret key

# Configuration
UPLOAD_FOLDER = 'uploads'
ALLOWED_EXTENSIONS = {'json'}
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER

# Ensure the upload folder exists
if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)

# Hard-coded admin credentials
ADMIN_USERNAME = 'admin'
ADMIN_PASSWORD = 'Daimon.1907'


def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS


# Login route for admin
@app.route('/login', methods=['GET', 'POST'])
def login():
    if request.method == 'POST':
        username = request.form.get('username')
        password = request.form.get('password')
        if username == ADMIN_USERNAME and password == ADMIN_PASSWORD:
            session['role'] = 'admin'
            session['username'] = username
            flash('Logged in as admin.')
            return redirect(url_for('index'))
        else:
            flash('Invalid admin credentials.')
            return redirect(url_for('login'))
    return render_template('login.html')


# Guest access route
@app.route('/guest')
def guest():
    session['role'] = 'guest'
    session['username'] = 'Guest'
    flash('Logged in as guest.')
    return redirect(url_for('index'))


# Logout route
@app.route('/logout')
def logout():
    session.clear()
    flash('Logged out.')
    return redirect(url_for('login'))


# Main index route
@app.route('/', methods=['GET', 'POST'])
def index():
    # If not logged in, redirect to login page
    if 'role' not in session:
        return redirect(url_for('login'))

    # Handle file upload (only admin is allowed)
    if request.method == 'POST':
        if session.get('role') != 'admin':
            flash('Only admin can upload files.')
            return redirect(url_for('index'))
        if 'jsonfile' not in request.files:
            flash('No file part.')
            return redirect(request.url)
        file = request.files['jsonfile']
        if file.filename == '':
            flash('No file selected.')
            return redirect(request.url)
        if file and allowed_file(file.filename):
            filename = secure_filename(file.filename)
            filepath = os.path.join(app.config['UPLOAD_FOLDER'], filename)
            file.save(filepath)
            flash(f'File "{filename}" successfully uploaded.')
            return redirect(url_for('index'))

    # List all uploaded JSON files
    files = os.listdir(app.config['UPLOAD_FOLDER'])
    files = [f for f in files if allowed_file(f)]
    return render_template('index.html', files=files)


# Route to view a file's contents
@app.route('/view/<filename>')
def view_file(filename):
    filepath = os.path.join(app.config['UPLOAD_FOLDER'], filename)
    if not os.path.exists(filepath):
        flash('File not found.')
        return redirect(url_for('index'))
    with open(filepath, 'r') as f:
        try:
            data = json.load(f)
        except Exception as e:
            data = {"error": "Invalid JSON", "details": str(e)}
    json_formatted = json.dumps(data, indent=4)
    return render_template('view.html', filename=filename, json_data=json_formatted)


# Route to delete a file (admin only)
@app.route('/delete/<filename>', methods=['POST'])
def delete_file(filename):
    if session.get('role') != 'admin':
        flash('Only admin can delete files.')
        return redirect(url_for('index'))
    filepath = os.path.join(app.config['UPLOAD_FOLDER'], filename)
    if os.path.exists(filepath):
        os.remove(filepath)
        flash(f'File "{filename}" deleted.')
    else:
        flash('File not found.')
    return redirect(url_for('index'))


if __name__ == '__main__':
    app.run(debug=True)
