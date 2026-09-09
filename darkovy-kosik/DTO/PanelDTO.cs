using darkovy_kosik.Models;

namespace darkovy_kosik.DTO;

public class PanelDTO {
    public Item Item { get; set; } = new Item();
    public List<Item> Items { get; set; }
}