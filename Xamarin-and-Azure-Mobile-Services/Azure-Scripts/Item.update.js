function update(item, user, request) {

    item.UserId = user.userId;

    request.execute();

}
