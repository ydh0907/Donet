const socketIO = require('socket.io')

module.exports = (server, app) => {
    const io = socketIO(server, { path: '/socket.io' })
    app.set('io', io);

    const room = io.of('/room');
    const chat = io.of('/chat');

    room.on('connection', (socket) => {
        console.log('entered room page')
        socket.on('disconnect', ()=>{
            console.log('exited room page')
        })
    })

    chat.on('connection', (socket) => {
        console.log('entered chat page')

        socket.on('join', (roomID) => {
            socket.join(roomID)
        })

        socket.on('disconnect', () => {
            console.log('exited chat page')
        })

        socket.on('exit', (roomID) => {
            socket.leave(roomID);
        })
    })
}