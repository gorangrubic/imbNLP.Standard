// --------------------------------------------------------------------------------------------------------------------
// <copyright file="dat_business.cs" company="imbVeles" >
//
// Copyright (C) 2018 imbVeles
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Project: imbNLP.PartOfSpeech
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
using System;

namespace imbNLP.PartOfSpeech.flags.data
{
    [Flags]
    public enum dat_business
    {
        none = 0,

        number = 1 << 0,

        address = 1 << 1,

        id = 1 << 2,

        contact = 1 << 3,

        mobile = 1 << 4,

        message = 1 << 5,

        email = 1 << 6,

        internet = 1 << 7,

        office = 1 << 8,

        hq = 1 << 9,

        personal = 1 << 10,

        town = 1 << 11,

        street = 1 << 12,

        name = 1 << 13,

        company = 1 << 14,

        legalForm = 1 << 15,

        root = 1 << 16,

        domain = 1 << 17,

        corporation = 1 << 18,

        limited = 1 << 19,

        vat = 1 << 20,

        industry = 1 << 21,

        registration = 1 << 22,

        document = 1 << 23,

        external = 1 << 24,

        lastName = 1 << 25,

        needle = 1 << 26,

        phone = 1 << 27,

        country = 1 << 28,

        currency = 1 << 29,

        bank = 1 << 30,

        copyright = 1 << 31,

        // < --- personal name

        personalName = personal | name,

        personalLastName = personal | lastName,

        // < ----- phone numbers

        phoneNumber = number | phone,

        phoneOfficeNumber = phoneNumber | office,

        phoneOfficeNumberHq = phoneOfficeNumber | hq,

        phoneOfficeNumberPersonal = phoneOfficeNumber | personal,

        phoneMobileNumber = phoneNumber | mobile,

        phoneMobileNumberHq = phoneMobileNumber | hq,

        phoneMobileNumberPersonal = phoneMobileNumber | personal,

        phoneFaxNumber = phoneOfficeNumber | message,

        // < ----------- name and address

        companyName = company | name,

        domainName = domain | name,

        rootDomainName = domainName | root,

        companyRootDomainName = company | rootDomainName,

        companyDomainName = company | domainName,

        countryName = country | name,

        townName = town | name | address,

        zipCode = town | number | address,

        streetName = street | name | address,

        streetNumber = street | number | address,

        // < ----------- email stuff

        emailAddress = email | address,

        emailAddressOffice = emailAddress | office,

        emailAddressForPerson = emailAddress | personal,

        // < ----------- legal form

        ltd = legalForm | limited,

        inc = legalForm | corporation,

        shop = legalForm | personal,

        // < -------- numbers

        vatNumber = vat | number | id,

        regNumber = registration | number | id,

        bankAccountNumber = bank | number | id,

        industryNumber = industry | number | id,

        // < ------ needles

        //legalFormNeedle = legalForm | needle,

        //ltdNeedle = legalFormNeedle | ltd,

        //incNeedle = legalFormNeedle | inc,

        //shopNeedle = legalFormNeedle | shop,

        vatNeedle = vat | needle,

        regNeedle = registration | needle,

        indNeedle = industry | needle,

        phoneOfficeNeedle = phone | office | needle,

        faxNeedle = phone | office | message | needle,

        mobileNeedle = phone | mobile | needle,

        emailNeedle = email | needle,

        addressNeedle = address | needle,

        streetNameNeedle = street | name | needle,

        zipNeedle = zipCode | needle,

        bankAccountNeedle = bank | id | needle,

        bankNameNeedle = bank | needle | name,

        copyrightNeedle = copyright | needle
    }
}