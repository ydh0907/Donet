import express from 'express'
import njk from 'nunjucks'

const port = 9990;
const app = express();

app.set('view engine', 'njk')
njk.configure('page', {
    express: app,
    watch: true
})

app.get('/', (req, res) => {
    res.render('main', { text: 'hi', place: 'enter path' })
})

app.listen(port, () => {
    console.log(`Server Running On localhost:${port}`)
})
