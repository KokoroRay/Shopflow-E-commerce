using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("pay_transaction")]
[Index("IdempotencyKey", Name = "UQ__pay_tran__A7BA59F4A93848BE", IsUnique = true)]
public partial class PayTransaction
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("order_id")]
    public long OrderId { get; set; }

    [Column("method")]
    public byte Method { get; set; }

    [Column("txn_type")]
    public byte TxnType { get; set; }

    [Column("amount", TypeName = "decimal(19, 4)")]
    public decimal Amount { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("attempt")]
    public int Attempt { get; set; }

    [Column("idempotency_key")]
    [StringLength(100)]
    [Unicode(false)]
    public string IdempotencyKey { get; set; } = null!;

    [Column("vnp_txn_ref")]
    [StringLength(64)]
    [Unicode(false)]
    public string? VnpTxnRef { get; set; }

    [Column("vnp_transaction_no")]
    [StringLength(64)]
    [Unicode(false)]
    public string? VnpTransactionNo { get; set; }

    [Column("vnp_bank_code")]
    [StringLength(20)]
    [Unicode(false)]
    public string? VnpBankCode { get; set; }

    [Column("vnp_pay_date")]
    [StringLength(20)]
    [Unicode(false)]
    public string? VnpPayDate { get; set; }

    [Column("vnp_card_type")]
    [StringLength(20)]
    [Unicode(false)]
    public string? VnpCardType { get; set; }

    [Column("secure_hash")]
    [StringLength(256)]
    [Unicode(false)]
    public string? SecureHash { get; set; }

    [Column("request_json")]
    public string? RequestJson { get; set; }

    [Column("response_json")]
    public string? ResponseJson { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("PayTransactions")]
    public virtual OrdOrder Order { get; set; } = null!;
}
