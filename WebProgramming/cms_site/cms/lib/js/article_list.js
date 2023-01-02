function show(count_on_page, articles){
    articles.forEach((article, index) => {
        if (index > (page * count_on_page)-1 || index < ((page - 1) * count_on_page)-1) {
            article.style.display = 'none';
        } 
        else {
            article.style.display = 'flex';
        }
    });
}

function next_page() {
    if (page < page_count_total) {
        page++;
        document.getElementById('count__of__pages').innerText = page + ' / ' + page_count_total;
        let articles = document.querySelectorAll('.article');
        show(article_count, articles);
    }
}

function previous_page() {
    if (page > 1) {
        page--;
        document.getElementById('count__of__pages').innerText = page + ' / ' + page_count_total;
        let articles = document.querySelectorAll('.article');
        show(article_count, articles);
    }
}

function close_dialog(){
    let dialog = document.getElementById('article__dialog');
    dialog.close();
}

function open_dialog(){
    let dialog = document.getElementById('article__dialog');
    if (typeof dialog.showModal === "function") {
        dialog.showModal();
    } else {
        console.log("The <dialog> API is not supported by this browser");
    }
}

function create_article(){
    let name = document.getElementById('form__input').value;
    console.log(name);
    if (name != '') {
        window.location.href = '/?page=article-create&name=' + name;
    }
}

document.getElementById('next__page').addEventListener('click', next_page);
document.getElementById('previous__page').addEventListener('click', previous_page);
document.getElementById('openDialogButton').addEventListener('click', open_dialog);
document.getElementById('closeDialog').addEventListener('click', close_dialog);
document.getElementById('submit').addEventListener('click', create_article);

const article_count = 10;

let page = 1;
let article_count_total = document.getElementsByClassName('article').length;
let page_count_total = Math.ceil(article_count_total / article_count);
let articles = document.querySelectorAll('.article');

show(article_count, articles);
document.getElementById('count__of__pages').innerText = page + ' / ' + page_count_total;