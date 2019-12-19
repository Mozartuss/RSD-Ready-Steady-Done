// -----------------------------------------------------------------------
// <copyright file="JsonErrorModel.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;

namespace TodoWebProjekt.Helper
{
    public class JsonErrorModel
    {
        public string status { get; set; }

        public IEnumerable<ErrorModel> formErrors { get; set; }
    }

    public class ErrorModel
    {
        public string key { get; set; }

        public IEnumerable<string> errors { get; set; }
    }
}
