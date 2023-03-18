// SPDX-License-Identifier: MIT
// CHECK https://wizard.openzeppelin.com/#erc721 to generate / customise your own ERC721 contract
pragma solidity >=0.6.0 <0.9.0;

import "./node_modules/contracts/token/ERC721/ERC721.sol";
import "./node_modules/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
import "./node_modules/contracts/token/ERC721/extensions/ERC721URIStorage.sol";
import "./node_modules/contracts/security/Pausable.sol";
import "./node_modules/contracts/access/Ownable.sol";
import "./node_modules/contracts/token/ERC721/extensions/ERC721Burnable.sol";
import "./node_modules/contracts/utils/cryptography/draft-EIP712.sol";
import "./node_modules/contracts/token/ERC721/extensions/draft-ERC721Votes.sol";
import "./node_modules/contracts/utils/Counters.sol";

contract NFTERC721 is
    ERC721,
    ERC721Enumerable,
    ERC721URIStorage,
    Pausable,
    Ownable,
    ERC721Burnable,
    EIP712,
    ERC721Votes
{
    using Counters for Counters.Counter;

    Counters.Counter private _tokenIdCounter;

    constructor(string memory name, string memory symbol)
        ERC721(name, symbol)
        EIP712(name, "1")
    {}

    function pause() public onlyOwner {
        _pause();
    }

    function unpause() public onlyOwner {
        _unpause();
    }

    function safeMint(address to, string memory uri) public onlyOwner {
        uint256 tokenId = _tokenIdCounter.current();
        _tokenIdCounter.increment();
        _safeMint(to, tokenId);
        _setTokenURI(tokenId, uri);
    }

    function _beforeTokenTransfer(
        address from,
        address to,
        uint256 tokenId,
        uint256 b
    ) internal override(ERC721, ERC721Enumerable) whenNotPaused {
        super._beforeTokenTransfer(from, to, tokenId, b = 1);
    }


    function _afterTokenTransfer(
        address from,
        address to,
        uint256 tokenId,
        uint256 a
    ) internal override(ERC721, ERC721Votes) {
        super._afterTokenTransfer(from, to, tokenId, a = 1);
    }

    function _burn(uint256 tokenId)
        internal
        override(ERC721, ERC721URIStorage)
    {
        super._burn(tokenId);
    }

    function tokenURI(uint256 tokenId)
        public
        view
        override(ERC721, ERC721URIStorage)
        returns (string memory)
    {
        return super.tokenURI(tokenId);
    }

    function supportsInterface(bytes4 interfaceId)
        public
        view
        override(ERC721, ERC721Enumerable)
        returns (bool)
    {
        return super.supportsInterface(interfaceId);
    }

    function getTokenId(address owner, uint256 index)
        public
        view
        returns (uint256)
    {
        return tokenOfOwnerByIndex(owner, index);
    }
}
