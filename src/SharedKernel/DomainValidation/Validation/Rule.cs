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

using DomainValidation.Interfaces.Specification;
using DomainValidation.Interfaces.Validation;

namespace DomainValidation.Validation;

public class Rule<TEntity> : IRule<TEntity>
{
    private readonly ISpecification<TEntity> _specification;

    public Rule(ISpecification<TEntity> spec, string errorCode, string errorMessage)
    {
        _specification = spec;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public Rule(ISpecification<TEntity> spec, string errorCode)
    {
        _specification = spec;
        ErrorCode = errorCode;
        ErrorMessage = string.Empty;
    }

    public string ErrorCode { get; }

    public string ErrorMessage { get; }

    public bool Validate(TEntity entity)
    {
        return _specification.IsSatisfiedBy(entity);
    }
}
