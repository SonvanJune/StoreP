﻿using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IUserService
{
   public IResult AddUser(CreateUserDto createUserDto);
   public IResult GetAllUsers();
   public IResult GetUserByRole(string roleCode);
   public IResult GetUserById(string id);
   public IResult Register(RegisterUserDto registerUserDto);
   public IResult CheckVerifyOfEmail(string toke);
   public IResult Login(LoginUserDto loginUserDto);
   public IResult GetUserByToken(string toke);
   public IResult VerifyUserByEmail(string toke);
   public IResult ForgetPasswordByEmail(string email);
   public IResult CheckResetCode(ResetCodeDto dto);
   public IResult ResetPasswordOfEmail(ResetPasswordDto dto);
   public IResult GoogleRegister(GoogleRegisterDto googleRegisterDto);
   public IResult GoogleLogin(GoogleLoginDto googleLoginDto);
   public IResult UpdateStatusUser(UpdateStatusUserDto dto);
}
