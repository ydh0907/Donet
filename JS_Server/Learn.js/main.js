const { log, error } = require('console');
var http = require('http');
var url = require('url');
var fs = require('fs');
var qs = require('querystring');
var templateObject = require('./lib/template.js')

var app = http.createServer(function(req, res){
    var queryData = url.parse(req.url, true).query;
    var pathName = url.parse(req.url, true).pathname;
    var PathUrl = req.url;

    if(pathName == '/'){
        if(queryData.id == undefined){
            fs.readdir('./page', function(error, filelist){
                var list = templateObject.List(filelist);
                var title = '메인 페이지'
                var data = 'empty'
                var template = templateObject.HTML(title, list, data, '');
                    res.writeHead(200, {'Content-Type':'text/html; charset=utf-8'})
                    res.end(template);
            });
        }
        else{
            fs.readdir('./page', function(error, filelist){
                var list = templateObject.List(filelist);
                fs.readFile(`page/${queryData.id}`, 'utf-8', function(err, data){
                    var title = queryData.id;
                    var deleteForm = 
                    `
                    <form action="process_delete" method = "post" id = "delete">
                        <input type="hidden" name="id" value="${title}">
                        <input type="button" value = "delete post" onclick="no_really();">
                    </form>
                    <script>
                    function no_really() {
                        if (confirm("정말로 삭제하시겠습니까?")) {
                            // 삭제 동작 수행
                            var form = document.getElementById("delete");
                            form.submit();
                        } else {
                            // 취소 동작 수행
                            window.history.back();
                        }
                    }
                    </script>
                    `

                    var template = templateObject.HTML(title, list, data, 
                        `<a href="/update?id=${queryData.id}">글수정</a> ${deleteForm}`);
                    res.writeHead(200, {'Content-Type':'text/html; charset=utf-8'})
                    res.end(template);
                    });
            })
        }
    }
    else if(pathName == '/create'){
        fs.readdir('./page', function(err, filelist){
            var title = '글쓰기 페이지';
            var list = templateObject.List(filelist);
            var data = `
            <form action="http://localhost:9977/process_create" method="post">
            <p>
                <input type = "text" name = "title" placeholder="title">
            </p>
            <p>
                <textarea name = "description" placeholder="description"></textarea>
            </p>
            <p>
                <input type="submit">
            </p>
            </form>
            `
            var template = templateObject.HTML(title, list, data, ``);
            res.writeHead(200, {'Content-Type':'text/html; charset=utf-8'})
            res.end(template);
        })
    }
    else if(pathName == '/update'){
        fs.readdir('./page', function(err, filelist){
            fs.readFile(`page/${queryData.id}`, 'utf-8', function(err, updateData){
                var title = '글수정 페이지';
                var list = templateObject.List(filelist);
                var data = `
                <form action="http://localhost:9977/process_update" method="post">
                <p>
                    <input type ="hidden" name = "id" value="${queryData.id}">
                    <input type = "text" name = "title" value="${queryData.id}">
                </p>
                <p>
                    <textarea name = "description" placeholder="description">${updateData}</textarea>
                </p>
                <p>
                    <input type="submit">
                </p>
                </form>
                `
                var template = templateObject.HTML(title, list, data, `<a href="/update?id=${queryData.id}">글수정</a>`);
                res.writeHead(200, {'Content-Type':'text/html; charset=utf-8'})
                res.end(template);
            })
        })
    }
    else if(pathName == '/process_create'){
        var body = "";
        req.on('data', function(data){
            body += data;
        })
        req.on("end", function(){
            var postData = qs.parse(body);
            var title = postData.title;
            var description = postData.description;

            fs.writeFile(`page/${title}`, description, 'utf-8', function(err){
                res.writeHead(302, {location: encodeURI(`/?id=${title}`)});
                res.end('');
            })
        })
    }
    else if(pathName == '/process_update'){
        var body = "";
        req.on('data', function(data){
            body += data;
        })
        req.on("end", function(){
            var postData = qs.parse(body);
            var title = postData.title;
            var description = postData.description;
            var id = postData.id;

            fs.rename(`page/${id}`, `page/${title}`, function(err){
                fs.writeFile(`page/${title}`, description, 'utf-8', function(err){
                    res.writeHead(302, {location: encodeURI(`/?id=${title}`)});
                    res.end('');
                })
            })
        })
    }
    else if(pathName == '/process_delete'){
        var body = "";
        req.on('data', function(data){
            body += data;
        })
        req.on("end", function(){
            var postData = qs.parse(body);
            var id = postData.id;

            fs.unlink(`page/${id}`, function(){
                res.writeHead(302, {location:'/'})
                res.end();
            })
        })
    }
    else{
        res.writeHead(404, {'Content-Type':'text/html; charset=utf-8'});
        res.end("404 나가")
    }
    // console.log(__dirname)
    // console.log(pathUrl);
    // res.end(fs.readFileSync(__dirname+pathUrl));
    
})
app.listen(9977);

app.on('listening', () => {
    console.log('server active on 9977 port')
})
app.on('error', (error) => {
    console.log(error);
})