import flask
import uuid

DB = {
    "1": {
        "id": "1",
        "name": "John",
    }
}

app = flask.Flask(__name__, static_folder="./static", static_url_path="/")

def main():
    app.run(port="8080", debug=True)

@app.route("/api/v1/student", methods=["GET", "POST"])
def get_student():
    if flask.request.method == "GET":
        return list(DB.values())
    else:
        content = flask.request.json
        id  = str(uuid.uuid4())
        content["id"] = id
        DB[id] = content
        return "OK"

@app.route("/api/v1/student/<id>", methods=["DELETE"])
def delete_student(id):
    del DB[id]
    return "OK"

if __name__ == "__main__":
    main() 