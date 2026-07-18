/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

/*
 * MIT License

 * Copyright (c) 2019 Henrique Dal Bello

 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace DomainValidation.Validation;

public class ValidationResult
{
    public ValidationResult()
    {
        Errors = new ValidationError[] { };
    }

    public string Message { get; set; }

    public bool IsValid => !Errors.Any();

    public IEnumerable<ValidationError> Errors { get; private set; }

    public void Add(ValidationError error)
    {
        var list = new List<ValidationError>(Errors) { error };
        SetErrors(list);
    }

    public void Add(params ValidationResult[] validationResults)
    {
        var list = new List<ValidationError>(Errors);
        foreach (var validation in validationResults)
            list.AddRange(validation.Errors);

        SetErrors(list);
    }

    public void Remove(ValidationError error)
    {
        var list = new List<ValidationError>(Errors);
        list.Remove(error);
        SetErrors(list);
    }

    private void SetErrors(List<ValidationError> errors)
    {
        Errors = errors;

        if (!IsValid)
            Message = errors[0].ErrorCode;
    }
}
