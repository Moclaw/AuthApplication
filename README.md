# AuthApplication API Documentation

## Register User API

**Endpoint:** `/api/users/register`

**Method:** `POST`

**Request Body:**
```json
{
  "username": "string",
  "password": "string",
  "passwordConfirm": "string",
  "email": "string",
  "phoneNumber": "string",
  "address": "string"
}
```
## Login API
**Endpoint:** `/api/users/login`

**Method:** `POST`

**Request Body:**

```json
{
  "userName": "string",
  "password": "string"
}
```
