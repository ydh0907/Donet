const express = require('express')
const session = require('express-session')
const fileStore = require('session-file-store')(session)
const dotenv = require('dotenv')
const nunjucks = require('nunjucks')
const app = express()
const webSocket = require('./socket')
const colorHash = require('color-hash').default;
const port = 3001

const indexRouter = require('./routes')

dotenv.config()
app.set('view engine', 'html')
nunjucks.configure('views', {
    express: app,
    watch: true
})

app.use(express.static('public'));
app.use(express.urlencoded({extended:false}));
app.use(session({
    secret: process.env.COOKIE_SECRET,
    resave: false,
    saveUninitialized: false,
    store: new fileStore()
}))

app.use((req, res, next) => {
    if (!req.session.color) {
        const sessionColor = new colorHash();
        req.session.color = sessionColor.hex(req.sessionID);
    }
    next();
})

app.use('/', indexRouter)

const server = app.listen(port, () => {
    console.log(`server listen on ${port}`);
})

webSocket(server, app)