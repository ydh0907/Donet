const express = require('express')
const app = express()
const port = 3000
const url = require('url');
const fs = require('fs');
const qs = require('querystring');
const templateObject = require('./lib/template.js')
const authStatus = require('./lib/auth.js')
const bodyParser = require('body-parser')
const cookieParser = require('cookie-parser');
const session = require('express-session')
const fileStore = require('session-file-store')(session);
const shortid = require("shortid");

const low = require('lowdb');
const FileSync = require("lowdb/adapters/FileSync.js")
const adapter = new FileSync('db.json');
const db = low(adapter);

db.defaults({ users: [], topics: [] }).write();

const authData = {
    email: 'dohee@gmail.com',
    password: 'doheeheehee',
    nickname: 'dohee'
}

app.use(bodyParser.urlencoded({ extended: false }))

app.use(express.static('public'));

// app.use(cookieParser())

app.use(session({
    secret: 'network',
    resave: false,
    saveUninitialized: true,
    store: new fileStore()
}))

const passport = require('passport');
const LocalStrategy = require('passport-local');

app.use(passport.initialize());
app.use(passport.session());

passport.serializeUser((user, done) => {
    console.log(`serialize: `, user);
    done(null, user.id);
})

passport.deserializeUser((id, done) => {
    console.log('deserialize: ', id);
    const user = db.get("users").find({ id: id }).value();
    done(null, user);
})

app.post('/process_login', passport.authenticate('local', {
    successRedirect: '/',
    failureRedirect: '/auth/login'
}));

passport.use(new LocalStrategy(function verify(username, password, cb) {
    console.log('local:', username, password);
    const user = db.get('users').find({ email: username, password: password }).value();
    if (user) {
        return cb(null, user);
    }
    return cb(null, false, { message: "not user" })
}));

app.use('/visit', (req, res, next) => {
    if (!req.session.views) {
        req.session.views = 0;
    }
    req.session.views += 1;
    next();
})

app.use((req, res, next) => {
    const filelist = db.get('topics').value();
    req.list = filelist;
    next();
})

app.use((req, res, next) => {
    console.log('it called all req.');
    next();
})

app.get('/auth/register', (req, res) => {
    const list = templateObject.List(req.list);
    const title = '회원가입 페이지';
    const data =
        `
            <form action="http://localhost:${port}/process_register" method="post">
            <p>
                <input type = "text" name = "username" placeholder="email">
            </p>
            <p>
                <input type = "password" name = "password" placeholder="password">
            </p>
            <p>
                <input type = "password" name = "password2" placeholder="password 확인">
            </p>
            <p>
                <input type = "text" name = "displayName" placeholder="닉네임">
            </p>
            <p>
                <input type="submit" value="회원가입">
            </p>
            </form>
        `
    const template = templateObject.HTML(title, list, data, ``, authStatus(req));
    res.send(template);
})

app.get('/auth/login', (req, res) => {
    const list = templateObject.List(req.list);
    const title = '로그인 페이지';
    const data =
        `
            <form action="http://localhost:${port}/process_login" method="post">
            <p>
                <input type = "text" name = "username" placeholder="email">
            </p>
            <p>
                <input type = "password" name = "password" placeholder="password">
            </p>
            <p>
                <input type="submit" value="로그인">
            </p>
            </form>
        `
    const template = templateObject.HTML(title, list, data, ``);
    res.send(template);
})

app.post('/process_register', (req, res) => {
    const postData = req.body
    const email = postData.username;
    const password = postData.password;
    const password2 = postData.password2;
    const displayName = postData.displayName;

    if (password !== password2) {
        res.redirect(`/auth/register`);
    }

    const user = { id: shortid.generate(), email: email, password: password, nickname: displayName };

    db.get('users').push(user).write();
    req.login(user, (err) => {
        return res.redirect('/');
    })
})

// app.post('/process_login', (req, res) => {
//     const postData = req.body
//     const email = postData.email;
//     const password = postData.password;

//     if(email === authData.email && password === authData.password){
//         req.session.is_logined = true;
//         req.session.nickname = authData.nickname;
//         req.session.save(() => {
//             res.redirect('/');
//         })
//     }
//     else{
//         res.send("failed");
//     }
// })

app.get('/', (req, res) => {
    const list = templateObject.List(req.list);
    const title = '메인 페이지'
    const data = `<p><img src="Tiger.png" style="width:300px"></p>`
    const template = templateObject.HTML(title, list, data, '', authStatus(req));
    //res.cookie('MyCookie', '윤도희')
    //console.log(req.cookies)
    res.send(template);
})

app.get('/page/:pageId', (req, res, next) => {
    const topic = db.get('topics').find({ id: req.params.pageId }).value();
    const writer = db.get('users').find({ id: topic.user_id }).value();
    const title = topic.title;
    const list = templateObject.List(req.list);
    const deleteForm =
        `
            <form action="/process_delete" method = "post" id = "delete">
                <input type="hidden" name="id" value="${topic.id}">
                <input type="button" value = "delete post" onclick="DeleteCheck();">
            </form>
            <script>
            function DeleteCheck() {
                if (confirm("정말로 삭제하시겠습니까?")) {
                    // 삭제 동작 수행
                    const form = document.getElementById("delete");
                    form.submit();
                } else {
                    // 취소 동작 수행
                    window.history.back();
                }
            }
            </script>
            `

    const template = templateObject.HTML(title, list, topic.description + `<p>by. ${writer.nickname}</p>`,
        `<a href="/update/${topic.id}">글수정</a> ${deleteForm}`, authStatus(req));
    res.send(template);
})

app.get('/create', (req, res) => {
    const list = templateObject.List(req.list);
    const title = '글쓰기 페이지';
    const data = `
            <form action="http://localhost:${port}/process_create" method="post">
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
    const template = templateObject.HTML(title, list, data, ``, authStatus(req));
    res.send(template);
})

app.post('/process_create', function (req, res) {
    const postData = req.body;
    const title = postData.title;
    const description = postData.description;

    const topic = { id: shortid.generate(), title: title, description: description, user_id: req.user.id };
    db.get('topics').push(topic).write();
    res.redirect(`/page/${topic.id}`);
})

app.get('/update/:pageId', (req, res) => {
    const topic = db.get('topics').find({ id: req.params.pageId }).value();
    const list = templateObject.List(req.list);
    const title = '글수정 페이지';
    const data = `
        <form action="http://localhost:3000/process_update" method="post">
        <p>
            <input type ="hidden" name = "id" value="${req.params.pageId}">
            <input type = "text" name = "title" value="${topic.title}">
        </p>
        <p>
            <textarea name = "description" placeholder="description">${topic.description}</textarea>
        </p>
        <p>
            <input type="submit">
        </p>
        </form>
        `

    const template = templateObject.HTML(title, list, data, `<a href="/update/${req.params.pageId}">글수정</a>`, authStatus(req));
    res.send(template);
})

app.post('/process_update', (req, res) => {
    const postData = req.body
    const title = postData.title;
    const description = postData.description;
    const id = postData.id;

    const topic = db.get('topics').find({ id: id }).value();
    if (topic.user_id !== req.user.id) {
        return res.redirect('/');
    }
    db.get('topics').find({ id: id }).assign({ title: title, description: description }).write();
    res.redirect(`/page/${id}`);
})

app.post('/process_delete', (req, res) => {
    const postData = req.body
    const id = postData.id;
    const topic = db.get('topics').find({ id: id }).value();

    if (topic.user_id !== req.user.id) {
        return res.redirect('/');
    }
    db.get('topics').remove({ id: id }).write();
    res.redirect('/');
})

app.get('/auth/logout', (req, res) => {
    req.session.destroy((err) => {
        res.redirect('/');
    })
})

app.get('/visit', (req, res) => {
    res.send(`${req.session.views}번 방문`);
})

app.use((req, res, next) => {
    res.status(404).send("404 NOT FOUND");
})

app.use((err, req, res, next) => {
    console.error(err.stack);
    res.status(500).send('안 알려 줄거다');
})

app.listen(port, () => {
    console.log(`Example app listening on port ${port}`)
})