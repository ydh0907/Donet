const express = require('express')

const port = 9997;

const app = express();

app.get('/', (req, res) => {
    res.send('hello')
})

app.listen(port, () => {
    console.log(`Server Running On localhost:${port}`)
})