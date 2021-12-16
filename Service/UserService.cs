﻿using DataModels.Entities;
using Repository.Interface;
using Service.Interface;
using System;
using System.Net;
using DataModels.VM.Common;
using DataModels.VM.User;
using DataModels.VM.Account;

namespace Service
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IInstructorTypeRepository _instructorTypeRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ICompanyRepository _companyRepository;

        public UserService(IUserRepository userRepository, IUserRoleRepository userRoleRepository
            , IInstructorTypeRepository instructorTypeRepository, ICountryRepository countryRepository,
            ICompanyRepository companyRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _instructorTypeRepository = instructorTypeRepository;
            _countryRepository = countryRepository;
            _companyRepository = companyRepository;
        }

        public CurrentResponse GetDetails(int id, int companyId, int roleId)
        {
            User user = _userRepository.FindByCondition(p => p.Id == id && p.IsDeleted != true);

            UserVM userVM = new UserVM();

            if (user != null)
            {
                userVM = ToBusinessObject(user);
            }

            userVM.Countries = _countryRepository.ListDropDownValues();
            userVM.InstructorTypes = _instructorTypeRepository.ListDropDownValues();
            userVM.UserRoles = _userRoleRepository.ListDropDownValues(roleId);

            if (companyId > 0)
            {
                userVM.CompanyId = companyId;
                userVM.CompanyName = _companyRepository.FindByCondition(p => p.Id == companyId).Name;
            }
            else
            {
                userVM.Companies = _companyRepository.ListDropDownValues();
            }

            CreateResponse(userVM, HttpStatusCode.OK, "");

            return _currentResponse;
        }

        public CurrentResponse Create(UserVM userVM)
        {
            try
            {
                User user = ToDataObject(userVM);
                user = _userRepository.Create(user);

                CreateResponse(user, HttpStatusCode.OK, "User added successfully");

                return _currentResponse;
            }
            catch (Exception exc)
            {
                CreateResponse(null, HttpStatusCode.InternalServerError, exc.ToString());

                return _currentResponse;
            }
        }

        public CurrentResponse Edit(UserVM userVM)
        {
            try
            {
                User user = ToDataObject(userVM);
                user = _userRepository.Edit(user);

                CreateResponse(user, HttpStatusCode.OK, "User updated successfully");

                return _currentResponse;
            }
            catch (Exception exc)
            {
                CreateResponse(null, HttpStatusCode.InternalServerError, exc.ToString());

                return _currentResponse;
            }
        }

        public CurrentResponse IsEmailExist(string email)
        {
            try
            {
                bool isEmailExist = _userRepository.IsEmailExist(email);

                CreateResponse(isEmailExist, HttpStatusCode.OK, "");

                return _currentResponse;
            }
            catch (Exception exc)
            {
                CreateResponse(null, HttpStatusCode.InternalServerError, exc.ToString());

                return _currentResponse;
            }
        }
        public CurrentResponse ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            try
            {
                bool isUserPasswordReset = _userRepository.ResetUserPassword(resetPasswordVM);

                if (isUserPasswordReset)
                {
                    CreateResponse(isUserPasswordReset, HttpStatusCode.OK, "Password reset successfully");
                }
                else
                {
                    CreateResponse(isUserPasswordReset, HttpStatusCode.OK, "Something went wrong. Try again later.");
                }

                return _currentResponse;
            }
            catch (Exception exc)
            {
                CreateResponse(null, HttpStatusCode.InternalServerError, exc.ToString());

                return _currentResponse;
            }
        }
        public CurrentResponse List(DatatableParams datatableParams)
        {
            try
            {
                var data = _userRepository.List(datatableParams);

                CreateResponse(data, HttpStatusCode.OK, "");

                return _currentResponse;
            }

            catch (Exception exc)
            {
                CreateResponse(null, HttpStatusCode.InternalServerError, exc.ToString());

                return _currentResponse;
            }
        }

        #region Object Mapping

        public User ToDataObject(UserVM userVM)
        {
            User user = new User();

            user.Id = userVM.Id;
            user.CompanyId = userVM.CompanyId;
            user.DateofBirth = userVM.DateofBirth;
            user.Email = userVM.Email;
            user.FirstName = userVM.FirstName;
            user.LastName = userVM.LastName;

            if (!string.IsNullOrWhiteSpace(userVM.Password))
            {
                user.Password = userVM.Password;
            }

            user.Phone = userVM.Phone;
            user.RoleId = userVM.RoleId;
            user.IsInstructor = userVM.IsInstructor;
            user.InstructorTypeId = (bool)userVM.IsInstructor ? userVM.InstructorTypeId : null;
            user.Gender = userVM.Gender;
            user.ExternalId = userVM.ExternalId;
            user.CountryId = userVM.CountryId;
            user.ExternalId = userVM.ExternalId;
            user.IsSendEmailInvite = userVM.IsSendEmailInvite;
            user.IsSendTextMessage = userVM.IsSendTextMessage;
            user.CreatedBy = userVM.CreatedBy;
            user.UpdatedBy = userVM.UpdatedBy;

            user.Gender = userVM.Gender;

            if (userVM.Id == 0)
            {
                user.CreatedOn = DateTime.UtcNow;
            }

            user.UpdatedOn = DateTime.UtcNow;

            return user;
        }

        public UserVM ToBusinessObject(User user)
        {
            UserVM userDetails = new UserVM();

            userDetails.Id = user.Id;
            userDetails.CompanyId = user.CompanyId;
            userDetails.DateofBirth = user.DateofBirth;
            userDetails.Email = user.Email;
            userDetails.FirstName = user.FirstName;
            userDetails.LastName = user.LastName;
            userDetails.Password = user.Password;
            userDetails.Phone = user.Phone;
            userDetails.RoleId = user.RoleId;
            userDetails.IsInstructor = user.IsInstructor;
            userDetails.InstructorTypeId = user.InstructorTypeId == null ? 0 : (int)user.InstructorTypeId;
            userDetails.Gender = user.Gender;
            userDetails.ExternalId = user.ExternalId;
            userDetails.CountryId = user.CountryId;
            userDetails.ExternalId = user.ExternalId;
            userDetails.IsSendEmailInvite = user.IsSendEmailInvite;
            userDetails.Gender = user.Gender;
            userDetails.IsSendTextMessage = user.IsSendTextMessage;

            return userDetails;
        }

        #endregion

        public CurrentResponse Delete(int id)
        {
            try
            {
                _userRepository.Delete(id);
                CreateResponse(true, HttpStatusCode.OK, "User deleted successfully.");

                return _currentResponse;
            }

            catch (Exception exc)
            {
                CreateResponse(false, HttpStatusCode.InternalServerError, exc.ToString());

                return _currentResponse;
            }
        }

        public CurrentResponse UpdateActiveStatus(int id, bool isActive)
        {
            try
            {
                _userRepository.UpdateActiveStatus(id, isActive);

                string message = "User activated successfully.";

                if (!isActive)
                {
                    message = "User deactivated successfully.";

                }

                CreateResponse(true, HttpStatusCode.OK, message);

                return _currentResponse;
            }

            catch (Exception exc)
            {
                CreateResponse(false, HttpStatusCode.InternalServerError, exc.ToString());

                return _currentResponse;
            }
        }

        public CurrentResponse GetFiltersValue()
        {
            try
            {
                UserFilterVM userFilterVM = new UserFilterVM();

                userFilterVM.Companies = _companyRepository.ListDropDownValues();

                CreateResponse(userFilterVM, HttpStatusCode.OK, "");

                return _currentResponse;
            }

            catch (Exception exc)
            {
                CreateResponse(new UserFilterVM(), HttpStatusCode.InternalServerError, exc.ToString());

                return _currentResponse;
            }
        }
    }
}
