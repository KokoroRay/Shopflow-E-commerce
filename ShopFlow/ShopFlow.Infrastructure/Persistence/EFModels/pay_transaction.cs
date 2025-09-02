using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class pay_transaction
{
    public long id { get; set; }

    public long order_id { get; set; }

    public byte method { get; set; }

    public byte txn_type { get; set; }

    public decimal amount { get; set; }

    public byte status { get; set; }

    public int attempt { get; set; }

    public string idempotency_key { get; set; } = null!;

    public string? vnp_txn_ref { get; set; }

    public string? vnp_transaction_no { get; set; }

    public string? vnp_bank_code { get; set; }

    public string? vnp_pay_date { get; set; }

    public string? vnp_card_type { get; set; }

    public string? secure_hash { get; set; }

    public string? request_json { get; set; }

    public string? response_json { get; set; }

    public DateTime created_at { get; set; }

    public virtual ord_order order { get; set; } = null!;
}
