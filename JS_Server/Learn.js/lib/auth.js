function authUI(req){
    let authStatus;
    if(req.user){
        authStatus = `${req.user.nickname} | <a href="/auth/logout">로그아웃</a>`
    }
    return authStatus;
}

module.exports = authUI;