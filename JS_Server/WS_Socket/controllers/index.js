const shortid = require('shortid')
const low = require('lowdb')
const FileSync = require('lowdb/adapters/FileSync')
const adapter = new FileSync('db.json')
const db = low(adapter)

db.defaults({ room: [], chat: [] }).write()

exports.renderMain = (req, res, next) => {
    try{
        const rooms = db.get('room').value();
        res.render('main', { rooms, title: 'GIF chatting' });
    }
    catch(error){
        console.error(error);
    }
}

exports.renderRoom = (req, res, next) => {
    res.render('room', {title: "GIF 채팅방 생성"})
}

exports.createRoom = (req, res, next) => {
    try{
        const newRoom = {id: shortid.generate(), title: req.body.title, max: req.body.max, owner: req.session.color, password: req.body.password}
        db.get('room').push(newRoom).write();

        const io = req.app.get('io');
        io.of('/room').emit('newRoom', newRoom);
        if(req.body.password){
            res.redirect(`/room/${newRoom.id}?password=${req.body.password}`)
        }
        else{
            res.redirect(`/room/${newRoom.id}`)
        }
    }
    catch(error){
        console.error(error)
    }
}

exports.enterRoom = (req, res, next) => {
    const room = db.get('room').find({id: req.params.id})
    if(!room){
        return res.redirect('/?error=존재하지 않는 방입니다.')
    }
    if(room.password && room.password !== req.query.password){
        return res.redirect('/?error=비번 틀림')
    }
    res.render('chat', {title: "GIF 채팅방", chats: [], user:req.session.color})
}

exports.removeRoom = (req, res, next) => {
    
}