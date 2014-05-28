function read(query, user, request) {

    query.where({ UserId: user.userId });

    request.execute();

}
