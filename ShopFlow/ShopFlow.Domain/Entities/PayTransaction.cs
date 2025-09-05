using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class PayTransaction
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public byte Method { get; set; }

    public byte TxnType { get; set; }

    public decimal Amount { get; set; }

    public byte Status { get; set; }

    public int Attempt { get; set; }

    public string IdempotencyKey { get; set; } = null!;

    public string? VnpTxnRef { get; set; }

    public string? VnpTransactionNo { get; set; }

    public string? VnpBankCode { get; set; }

    public string? VnpPayDate { get; set; }

    public string? VnpCardType { get; set; }

    public string? SecureHash { get; set; }

    public string? RequestJson { get; set; }

    public string? ResponseJson { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual OrdOrder Order { get; set; } = null!;
}
