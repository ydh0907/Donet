let templateObject = {
    HTML:function (title, list, data, control, authUI = '<a href="/auth/login">로그인</a> | <a href="/auth/register">회원가입</a>'){
        return `<!DOCTYPE html>
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>${title}</title>
        </head>
        <body>
        ${authUI}
        <h1>${title}</h1>
        <h2><a href="/">리스트로 가기</a></h2>
            ${list}
            <p>
                <a href="/create">글쓰기</a>
                ${control}
            </p>
            <p>
                ${data}
            </p>
        </body>
        </html>`;
    },
    List:function (filelist){
        let list = '<ul>';
        let i = 0;
        while(i < filelist.length){
            list += `<li><a href="/page/${filelist[i].id}">${filelist[i].title}</a></li>`
            i++;
        }
        list += '</ul>';

        return list;
    }
}

module.exports = templateObject;