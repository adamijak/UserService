# UserService
![tests](https://github.com/adamijak/UserService/actions/workflows/tests.yml/badge.svg)

## Get all users
```HTTP
GET /users/ HTTP/1.1
Host: {{host}}
```

## Add new user
```HTTP
POST /users HTTP/1.1
Content-Type: application/json
Host: {{host}}
Content-Length: 112

{
    "id": "id1",
    "email": "david@email",
    "fullName": "David Small",
    "birthDate": "2023-08-11T00:00:00+02:00"
}
```

## Get user
```HTTP
GET /users/{{id}} HTTP/1.1
Content-Type: application/json
Host: {{host}}
```

## Update user
```HTTP
PUT /users/{{id}} HTTP/1.1
Content-Type: application/json
Host: {{host}}
Content-Length: 28

{
    "email": "david@email",
    "fullName": "David Big",
    "birthDate": "2023-08-11T00:00:00+02:00"
}
```

## Delete user
```HTTP
DELETE /users/{{id}} HTTP/1.1
Content-Type: application/json
Host: {{host}}
```
